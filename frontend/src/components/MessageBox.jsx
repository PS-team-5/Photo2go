function MessageBox({ message, type = "info", onClose }) {
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
                    aria-label="Close notification"
                    title="Close"
                >
                    ×
                </button>
            ) : null}
        </div>
    );
}

export default MessageBox;
