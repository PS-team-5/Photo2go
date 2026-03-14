import { useState } from "react";
import "./App.css";

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
        } catch (error) {
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
        } catch (error) {
            setMessage("Could not connect to backend");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="auth-page">
            <div className="auth-card">
                <h1>Photo2Go</h1>
                <p className="subtitle">User authentication</p>

                <div className="switch-buttons">
                    <button
                        className={mode === "login" ? "active" : ""}
                        onClick={() => {
                            setMode("login");
                            setMessage("");
                        }}
                        type="button"
                    >
                        Login
                    </button>
                    <button
                        className={mode === "register" ? "active" : ""}
                        onClick={() => {
                            setMode("register");
                            setMessage("");
                        }}
                        type="button"
                    >
                        Register
                    </button>
                </div>

                {mode === "login" ? (
                    <form onSubmit={handleLogin} className="auth-form">
                        <input
                            type="text"
                            name="username"
                            placeholder="Username"
                            value={loginData.username}
                            onChange={handleLoginChange}
                            required
                        />
                        <input
                            type="email"
                            name="email"
                            placeholder="Email"
                            value={loginData.email}
                            onChange={handleLoginChange}
                            required
                        />
                        <input
                            type="password"
                            name="password"
                            placeholder="Password"
                            value={loginData.password}
                            onChange={handleLoginChange}
                            required
                        />
                        <button type="submit" disabled={loading}>
                            {loading ? "Logging in..." : "Login"}
                        </button>
                    </form>

                ) : (
                        <form onSubmit={handleRegister} className="auth-form">
                            <input
                                type="text"
                                name="username"
                                placeholder="Username"
                                value={registerData.username}
                                onChange={handleRegisterChange}
                                required
                            />
                            <input
                                type="email"
                                name="email"
                                placeholder="Email"
                                value={registerData.email}
                                onChange={handleRegisterChange}
                                required
                            />
                            <input
                                type="password"
                                name="password"
                                placeholder="Password"
                                value={registerData.password}
                                onChange={handleRegisterChange}
                                required
                            />
                            <button type="submit" disabled={loading}>
                                {loading ? "Registering..." : "Register"}
                            </button>
                        </form>

                )}

                {message && <p className="message">{message}</p>}
            </div>
        </div>
    );
}

export default App;
