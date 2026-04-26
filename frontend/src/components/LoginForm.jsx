import { useI18n } from "../i18n/useI18n";

function LoginForm({ loginData, handleLoginChange, handleLogin, loading }) {
    const { t } = useI18n();

    return (
        <form onSubmit={handleLogin} className="auth-form">
            <input
                type="text"
                name="username"
                placeholder={t("common.username")}
                value={loginData.username}
                onChange={handleLoginChange}
                required
            />
            <input
                type="email"
                name="email"
                placeholder={t("common.email")}
                value={loginData.email}
                onChange={handleLoginChange}
                required
            />
            <input
                type="password"
                name="password"
                placeholder={t("common.password")}
                value={loginData.password}
                onChange={handleLoginChange}
                required
            />
            <button type="submit" disabled={loading}>
                {loading ? t("auth.loginLoading") : t("common.login")}
            </button>
        </form>
    );
}

export default LoginForm;
