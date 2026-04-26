import { useI18n } from "../i18n/useI18n";

function AuthTabs({ mode, setMode, setMessage }) {
    const { t } = useI18n();

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
                {t("common.login")}
            </button>
            <button
                className={mode === "register" ? "active" : ""}
                onClick={() => {
                    setMode("register");
                    setMessage("");
                }}
                type="button"
            >
                {t("common.register")}
            </button>
        </div>
    );
}

export default AuthTabs;
