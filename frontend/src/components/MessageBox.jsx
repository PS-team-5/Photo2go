import { useI18n } from "../i18n/useI18n";

function MessageBox({ message, type = "info", onClose }) {
    const { t } = useI18n();

    if (!message) return null;

    const safeType =
        type === "success" || type === "error" || type === "info"
            ? type
            : "info";

    return (
        <div
            className={`toast toast-${safeType}`}
            role={safeType === "error" ? "alert" : "status"}
            aria-live={safeType === "error" ? "assertive" : "polite"}
        >
            <div className="toast-body">{message}</div>
            {onClose ? (
                <button
                    type="button"
                    className="toast-close"
                    onClick={onClose}
                    aria-label={t("common.close")}
                    title={t("common.close")}
                >
                    ×
                </button>
            ) : null}
        </div>
    );
}

export default MessageBox;
