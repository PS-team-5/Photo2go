import { useEffect, useState } from "react";
import "./App.css";
import AuthTabs from "./components/AuthTabs";
import LoginForm from "./components/LoginForm";
import RegisterForm from "./components/RegisterForm";
import MessageBox from "./components/MessageBox";
import UploadForm from "./components/UploadForm";

const USER_API_BASE_URL = "http://localhost:5218/api/user";
const ANALYZE_IMAGE_URL = "http://localhost:5218/analyze-image";
const AUTH_STORAGE_KEY = "photo2go-user";

async function readResponsePayload(response) {
    const contentType = response.headers.get("content-type") || "";

    if (contentType.includes("application/json")) {
        return response.json().catch(() => null);
    }

    const text = await response.text().catch(() => "");
    return text ? { message: text } : null;
}

function App() {
    const [mode, setMode] = useState("login");
    const [messageState, setMessageState] = useState({ text: "", type: "info" });
    const [loading, setLoading] = useState(false);
    const [loggedInUser, setLoggedInUser] = useState(null);
    const [selectedFile, setSelectedFile] = useState(null);
    const [analysisResult, setAnalysisResult] = useState(null);

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
                setMessage(responseText || "Login failed", "error");
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
            setMessage("Login successful. You can upload a photo now.", "success");
        } catch {
            setMessage("Could not connect to backend", "error");
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
                setMessage("Registration successful. Please log in.", "success");
                setRegisterData({
                    username: "",
                    email: "",
                    password: "",
                });
            } else {
                setMessage(text || "Registration failed", "error");
            }
        } catch {
            setMessage("Could not connect to backend", "error");
        } finally {
            setLoading(false);
        }
    };

    const handleAnalyzeImage = async (e) => {
        e.preventDefault();

        if (!selectedFile) {
            setMessage("Please choose an image first.", "error");
            return;
        }

        setLoading(true);
        setMessage("");
        setAnalysisResult(null);

        try {
            const formData = new FormData();
            formData.append("image", selectedFile);

            const response = await fetch(ANALYZE_IMAGE_URL, {
                method: "POST",
                body: formData,
            });

            const data = await readResponsePayload(response);

            if (!response.ok) {
                setMessage(
                    data?.message || `Image analysis failed (HTTP ${response.status})`,
                    "error",
                );
                return;
            }

            setAnalysisResult(data);
            const routeMessage = data?.routeMessage || data?.message || "Image analyzed successfully.";
            const routeGenerated = data?.routeGenerated;
            setMessage(
                routeMessage,
                routeGenerated === false ? "error" : "success",
            );
        } catch {
            setMessage("Could not connect to image analysis service", "error");
        } finally {
            setLoading(false);
        }
    };

    const handleLogout = () => {
        setLoggedInUser(null);
        setSelectedFile(null);
        setAnalysisResult(null);
        setMessage("You have been logged out.", "info");
        localStorage.removeItem(AUTH_STORAGE_KEY);
        setLoginData({
            username: "",
            email: "",
            password: "",
        });
    };

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
                            <h1 className="auth-title">Photo2Go</h1>
                            <p className="subtitle">
                                Logged in as <strong>{loggedInUser.username}</strong>
                            </p>
                        </div>
                        <button
                            type="button"
                            className="secondary-button"
                            onClick={handleLogout}
                        >
                            Logout
                        </button>
                    </div>

                    <UploadForm
                        selectedFile={selectedFile}
                        handleFileChange={handleFileChange}
                        handleAnalyzeImage={handleAnalyzeImage}
                        loading={loading}
                        analysisResult={analysisResult}
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
                <h1 className="auth-title">Photo2Go</h1>
                <p className="subtitle">User authentication</p>

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
