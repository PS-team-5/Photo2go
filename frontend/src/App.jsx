import { useEffect, useState } from "react";
import "./App.css";
import AuthTabs from "./components/AuthTabs";
import LoginForm from "./components/LoginForm";
import RegisterForm from "./components/RegisterForm";
import MessageBox from "./components/MessageBox";
import UploadForm from "./components/UploadForm";
import { useI18n } from "./i18n/useI18n";
import { LANGUAGE_OPTIONS } from "./i18n/translations";

const USER_API_BASE_URL = "http://localhost:5218/api/user";
const ANALYZE_IMAGE_URL = "http://localhost:5218/analyze-image";
const ANALYZE_IMAGE_FEEDBACK_URL = "http://localhost:5218/analyze-image/feedback";
const ANALYZE_IMAGE_TIMEOUT_MS = 15_000;
const AUTH_STORAGE_KEY = "photo2go-user";
const THEME_STORAGE_KEY = "photo2go-theme";
const DARK_THEME_MEDIA_QUERY = "(prefers-color-scheme: dark)";
const VILNIUS_CITY_NAME = "vilnius";

function getSystemTheme() {
    if (typeof window === "undefined") {
        return "light";
    }

    return window.matchMedia(DARK_THEME_MEDIA_QUERY).matches ? "dark" : "light";
}

function getStoredTheme() {
    if (typeof window === "undefined") {
        return null;
    }

    const storedTheme = window.localStorage.getItem(THEME_STORAGE_KEY);

    if (storedTheme === "light" || storedTheme === "dark") {
        return storedTheme;
    }

    return null;
}

async function readResponsePayload(response) {
    const contentType = response.headers.get("content-type") || "";

    if (contentType.includes("application/json")) {
        return response.json().catch(() => null);
    }

    const text = await response.text().catch(() => "");
    return text ? { message: text } : null;
}

function normalizeText(value) {
    return String(value || "")
        .trim()
        .toLocaleLowerCase("lt-LT");
}

function getRouteBlockingError(data, t) {
    const analysis = data?.analysis;
    const routeGenerated = data?.routeGenerated;
    const city = normalizeText(analysis?.city);

    if (!analysis?.name || !analysis?.objectType) {
        return t("messages.routeOnlyForVilnius");
    }

    if (!city) {
        return t("messages.routeOnlyForVilnius");
    }

    if (city !== VILNIUS_CITY_NAME) {
        return t("messages.routeOnlyForVilnius");
    }

    if (routeGenerated === false) {
        return t("messages.routeNotFound");
    }

    return "";
}

function App() {
    const { language, setLanguage, t } = useI18n();
    const [mode, setMode] = useState("login");
    const [messageState, setMessageState] = useState({ text: "", type: "info" });
    const [loading, setLoading] = useState(false);
    const [loggedInUser, setLoggedInUser] = useState(null);
    const [selectedFile, setSelectedFile] = useState(null);
    const [analysisResult, setAnalysisResult] = useState(null);
    const [selectedFeedback, setSelectedFeedback] = useState("");
    const [feedbackLoading, setFeedbackLoading] = useState(false);
    const [routeHistory, setRouteHistory] = useState([]);
    const [routesLoading, setRoutesLoading] = useState(false);
    const [routesError, setRoutesError] = useState("");
    const [theme, setTheme] = useState(() => getStoredTheme() || getSystemTheme());
    const [hasSavedTheme, setHasSavedTheme] = useState(() => Boolean(getStoredTheme()));

    const setMessage = (text, type = "info") => {
        setMessageState({ text, type });
    };

    useEffect(() => {
        if (!messageState.text) {
            return;
        }

        // Auto-hide info/success notifications. Keep errors until dismissed.
        if (messageState.type === "error") {
            return;
        }

        const timerId = window.setTimeout(() => {
            setMessageState({ text: "", type: "info" });
        }, 4000);

        return () => window.clearTimeout(timerId);
    }, [messageState.text, messageState.type]);

    const [loginData, setLoginData] = useState({
        username: "",
        email: "",
        password: "",
    });

    const [registerData, setRegisterData] = useState({
        username: "",
        email: "",
        password: "",
    });

    useEffect(() => {
        const savedUser = localStorage.getItem(AUTH_STORAGE_KEY);
        if (!savedUser) {
            return;
        }

        try {
            setLoggedInUser(JSON.parse(savedUser));
        } catch {
            localStorage.removeItem(AUTH_STORAGE_KEY);
        }
    }, []);

    useEffect(() => {
        document.documentElement.dataset.theme = theme;
        document.documentElement.style.colorScheme = theme;
    }, [theme]);

    useEffect(() => {
        if (hasSavedTheme || typeof window === "undefined") {
            return undefined;
        }

        const mediaQuery = window.matchMedia(DARK_THEME_MEDIA_QUERY);
        const handleChange = (event) => {
            setTheme(event.matches ? "dark" : "light");
        };

        handleChange(mediaQuery);

        if (typeof mediaQuery.addEventListener === "function") {
            mediaQuery.addEventListener("change", handleChange);

            return () => {
                mediaQuery.removeEventListener("change", handleChange);
            };
        }

        mediaQuery.addListener(handleChange);

        return () => {
            mediaQuery.removeListener(handleChange);
        };
    }, [hasSavedTheme]);

    useEffect(() => {
        if (!loggedInUser?.id) {
            setRouteHistory([]);
            setRoutesError("");
            setRoutesLoading(false);
            return;
        }

        let isCancelled = false;

        const loadRoutes = async () => {
            setRoutesLoading(true);
            setRoutesError("");

            try {
                const response = await fetch(
                    `${USER_API_BASE_URL}/${loggedInUser.id}/routes`,
                );
                const data = await readResponsePayload(response);

                if (isCancelled) {
                    return;
                }

                if (!response.ok) {
                    setRouteHistory([]);
                    setRoutesError(data?.message || t("messages.loadRoutesFailed"));
                    return;
                }

                setRouteHistory(Array.isArray(data) ? data : []);
            } catch {
                if (!isCancelled) {
                    setRouteHistory([]);
                    setRoutesError(t("messages.loadRoutesUnavailable"));
                }
            } finally {
                if (!isCancelled) {
                    setRoutesLoading(false);
                }
            }
        };

        loadRoutes();

        return () => {
            isCancelled = true;
        };
    }, [loggedInUser?.id, t]);

    const refreshRouteHistory = async (userId) => {
        if (!userId) {
            setRouteHistory([]);
            setRoutesError("");
            return;
        }

        setRoutesLoading(true);
        setRoutesError("");

        try {
            const response = await fetch(`${USER_API_BASE_URL}/${userId}/routes`);
            const data = await readResponsePayload(response);

            if (!response.ok) {
                setRouteHistory([]);
                setRoutesError(data?.message || t("messages.loadRoutesFailed"));
                return;
            }

            setRouteHistory(Array.isArray(data) ? data : []);
        } catch {
            setRouteHistory([]);
            setRoutesError(t("messages.loadRoutesUnavailable"));
        } finally {
            setRoutesLoading(false);
        }
    };

    const handleLoginChange = (e) => {
        const { name, value } = e.target;
        setLoginData((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    const handleRegisterChange = (e) => {
        const { name, value } = e.target;
        setRegisterData((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    const handleFileChange = (e) => {
        const file = e.target.files?.[0] ?? null;
        setSelectedFile(file);
        setAnalysisResult(null);
        setSelectedFeedback("");
        setMessage("");
    };

    const handleLogin = async (e) => {
        e.preventDefault();
        setLoading(true);
        setMessage("");

        try {
            const response = await fetch(`${USER_API_BASE_URL}/login`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(loginData),
            });

            const responseText = await response.text();

            if (!response.ok) {
                setMessage(responseText || t("messages.loginFailed"), "error");
                return;
            }

            const user = JSON.parse(responseText);
            const safeUser = {
                id: user.id,
                username: user.username,
                email: user.email,
            };

            setLoggedInUser(safeUser);
            localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(safeUser));
            setAnalysisResult(null);
            setSelectedFile(null);
            setMessage(t("messages.loginSuccess"), "success");
        } catch {
            setMessage(t("messages.backendUnavailable"), "error");
        } finally {
            setLoading(false);
        }
    };

    const handleRegister = async (e) => {
        e.preventDefault();
        setLoading(true);
        setMessage("");

        try {
            const response = await fetch(`${USER_API_BASE_URL}/register`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(registerData),
            });

            const text = await response.text();

            if (response.ok) {
                setMode("login");
                setMessage(t("messages.registerSuccess"), "success");
                setRegisterData({
                    username: "",
                    email: "",
                    password: "",
                });
            } else {
                setMessage(text || t("messages.registerFailed"), "error");
            }
        } catch {
            setMessage(t("messages.backendUnavailable"), "error");
        } finally {
            setLoading(false);
        }
    };

    const handleAnalyzeImage = async (e) => {
        e.preventDefault();

        if (!selectedFile) {
            setMessage(t("messages.chooseImageFirst"), "error");
            return;
        }

        setLoading(true);
        setMessage("");
        setAnalysisResult(null);
        setSelectedFeedback("");

        const abortController = new AbortController();
        let didTimeout = false;
        const timeoutId = window.setTimeout(() => {
            didTimeout = true;
            abortController.abort();
        }, ANALYZE_IMAGE_TIMEOUT_MS);

        try {
            const formData = new FormData();
            formData.append("userId", String(loggedInUser.id));
            formData.append("image", selectedFile);

            const response = await fetch(ANALYZE_IMAGE_URL, {
                method: "POST",
                body: formData,
                signal: abortController.signal,
            });

            const data = await readResponsePayload(response);

            if (!response.ok) {
                setMessage(
                    data?.message ||
                        t("messages.imageAnalysisFailedHttp", {
                            status: response.status,
                        }),
                    "error",
                );
                return;
            }

            const routeBlockingError = getRouteBlockingError(data, t);

            if (routeBlockingError) {
                setAnalysisResult({
                    ...data,
                    routeGenerated: false,
                    similarLocations: [],
                });
                setMessage(routeBlockingError, "error");
                return;
            }

            setAnalysisResult(data);
            setMessage(t("messages.imageAnalysisSuccess"), "success");
            await refreshRouteHistory(loggedInUser.id);
        } catch (error) {
            if (didTimeout || error?.name === "AbortError") {
                setMessage(t("messages.routeGenerationTimeout"), "error");
                return;
            }

            setMessage(t("messages.imageAnalysisUnavailable"), "error");
        } finally {
            window.clearTimeout(timeoutId);
            setLoading(false);
        }
    };

    const handleRecommendationFeedback = async (feedback) => {
        const detectedLocationId = analysisResult?.detectedLocationId;

        if (!detectedLocationId) {
            setMessage(t("messages.feedbackLocationMissing"), "error");
            return;
        }

        setFeedbackLoading(true);
        setSelectedFeedback(feedback);
        setMessage("");

        try {
            const response = await fetch(ANALYZE_IMAGE_FEEDBACK_URL, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    detectedLocationId,
                    feedback,
                    userId: loggedInUser?.id ?? null,
                    currentConfidence: analysisResult?.analysis?.confidence ?? null,
                }),
            });

            const data = await readResponsePayload(response);

            if (!response.ok) {
                setSelectedFeedback("");
                setMessage(
                    data?.message || t("messages.feedbackSaveFailed"),
                    "error",
                );
                return;
            }

            setAnalysisResult((current) =>
                current
                    ? {
                          ...current,
                          analysis: current.analysis
                              ? {
                                    ...current.analysis,
                                    confidence:
                                        typeof data?.adjustedConfidence === "number"
                                            ? data.adjustedConfidence
                                            : Number(current.analysis.confidence || 0),
                                }
                              : current.analysis,
                      }
                    : current,
            );
            setMessage(t("messages.feedbackSaved"), "success");
        } catch {
            setSelectedFeedback("");
            setMessage(t("messages.feedbackUnavailable"), "error");
        } finally {
            setFeedbackLoading(false);
        }
    };

    const handleLogout = () => {
        setLoggedInUser(null);
        setSelectedFile(null);
        setAnalysisResult(null);
        setSelectedFeedback("");
        setRouteHistory([]);
        setRoutesError("");
        setRoutesLoading(false);
        setMessage(t("messages.logoutSuccess"), "info");
        localStorage.removeItem(AUTH_STORAGE_KEY);
        setLoginData({
            username: "",
            email: "",
            password: "",
        });
    };

    const handleThemeToggle = () => {
        setTheme((currentTheme) => {
            const nextTheme = currentTheme === "dark" ? "light" : "dark";
            window.localStorage.setItem(THEME_STORAGE_KEY, nextTheme);
            return nextTheme;
        });
        setHasSavedTheme(true);
    };

    const nextThemeLabel = theme === "dark"
        ? t("common.light")
        : t("common.dark");

    const themeToggle = (
        <button
            type="button"
            className="secondary-button theme-toggle"
            onClick={handleThemeToggle}
            aria-label={t("common.switchTheme", { theme: nextThemeLabel })}
            title={t("common.switchTheme", { theme: nextThemeLabel })}
        >
            <span className="theme-toggle-label">{t("common.theme")}</span>
            <span className="theme-toggle-value">
                {theme === "dark" ? t("common.dark") : t("common.light")}
            </span>
        </button>
    );

    const languageSelect = (
        <label className="preference-field">
            <span className="preference-label">{t("common.language")}</span>
            <select
                className="filter-select preference-select"
                value={language}
                onChange={(event) => setLanguage(event.target.value)}
                aria-label={t("common.language")}
            >
                {LANGUAGE_OPTIONS.map((option) => (
                    <option key={option.value} value={option.value}>
                        {option.label}
                    </option>
                ))}
            </select>
        </label>
    );

    if (loggedInUser) {
        return (
            <div className="auth-page">
                <MessageBox
                    message={messageState.text}
                    type={messageState.type}
                    onClose={() => setMessage("")}
                />
                <div className="auth-card app-card">
                    <div className="user-bar">
                        <div>
                            <h1 className="auth-title">{t("common.brand")}</h1>
                            <p className="subtitle">
                                {t("common.loggedInAsLabel")}{" "}
                                <strong>{loggedInUser.username}</strong>
                            </p>
                        </div>
                        <div className="header-actions">
                            {languageSelect}
                            {themeToggle}
                            <button
                                type="button"
                                className="secondary-button"
                                onClick={handleLogout}
                            >
                                {t("common.logout")}
                            </button>
                        </div>
                    </div>

                    <UploadForm
                        selectedFile={selectedFile}
                        handleFileChange={handleFileChange}
                        handleAnalyzeImage={handleAnalyzeImage}
                        loading={loading}
                        analysisResult={analysisResult}
                        selectedFeedback={selectedFeedback}
                        feedbackLoading={feedbackLoading}
                        onFeedbackSelect={handleRecommendationFeedback}
                        routeHistory={routeHistory}
                        routesLoading={routesLoading}
                        routesError={routesError}
                    />
                </div>
            </div>
        );
    }

    return (
        <div className="auth-page">
            <MessageBox
                message={messageState.text}
                type={messageState.type}
                onClose={() => setMessage("")}
            />
            <div className="auth-card">
                <div className="card-topbar">
                    <div className="header-actions">
                        {languageSelect}
                        {themeToggle}
                    </div>
                </div>
                <h1 className="auth-title">{t("common.brand")}</h1>
                <p className="subtitle">{t("auth.subtitle")}</p>

                <AuthTabs mode={mode} setMode={setMode} setMessage={setMessage} />

                {mode === "login" ? (
                    <LoginForm
                        loginData={loginData}
                        handleLoginChange={handleLoginChange}
                        handleLogin={handleLogin}
                        loading={loading}
                    />
                ) : (
                    <RegisterForm
                        registerData={registerData}
                        handleRegisterChange={handleRegisterChange}
                        handleRegister={handleRegister}
                        loading={loading}
                    />
                )}
            </div>
        </div>
    );
}

export default App;
