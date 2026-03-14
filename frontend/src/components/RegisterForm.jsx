function RegisterForm({
    registerData,
    handleRegisterChange,
    handleRegister,
    loading,
}) {
    return (
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
    );
}

export default RegisterForm;
