import { useState } from "react";
import "./App.css";
import AuthTabs from "./components/AuthTabs";
import LoginForm from "./components/LoginForm";
import RegisterForm from "./components/RegisterForm";
import MessageBox from "./components/MessageBox";

const API_BASE_URL = "http://localhost:5218/api/user";

function App() {
    const [mode, setMode] = useState("login");
    const [message, setMessage] = useState("");
    const [loading, setLoading] = useState(false);

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

    const handleLogin = async (e) => {
        e.preventDefault();
        setLoading(true);
        setMessage("");

        try {
            const response = await fetch(`${API_BASE_URL}/login`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(loginData),
            });

            const text = await response.text();

            if (response.ok) {
                setMessage("Login successful");
            } else {
                setMessage(text || "Login failed");
            }
        } catch {
            setMessage("Could not connect to backend");
        } finally {
            setLoading(false);
        }
    };

    const handleRegister = async (e) => {
        e.preventDefault();
        setLoading(true);
        setMessage("");

        try {
            const response = await fetch(`${API_BASE_URL}/register`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(registerData),
            });

            const text = await response.text();

            if (response.ok) {
                setMessage("Registration successful");
            } else {
                setMessage(text || "Registration failed");
            }
        } catch {
            setMessage("Could not connect to backend");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="auth-page">
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

                <MessageBox message={message} />
            </div>
        </div>
    );
}

export default App;
