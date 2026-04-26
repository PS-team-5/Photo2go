import { useI18n } from "../i18n/useI18n";

function RegisterForm({
    registerData,
    handleRegisterChange,
    handleRegister,
    loading,
}) {
    const { t } = useI18n();

    return (
        <form onSubmit={handleRegister} className="auth-form">
            <input
                type="text"
                name="username"
                placeholder={t("common.username")}
                value={registerData.username}
                onChange={handleRegisterChange}
                required
            />
            <input
                type="email"
                name="email"
                placeholder={t("common.email")}
                value={registerData.email}
                onChange={handleRegisterChange}
                required
            />
            <input
                type="password"
                name="password"
                placeholder={t("common.password")}
                value={registerData.password}
                onChange={handleRegisterChange}
                required
            />
            <button type="submit" disabled={loading}>
                {loading ? t("auth.registerLoading") : t("common.register")}
            </button>
        </form>
    );
}

export default RegisterForm;
