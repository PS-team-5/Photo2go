function AuthTabs({ mode, setMode, setMessage }) {
    return (
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
    );
}

export default AuthTabs;
