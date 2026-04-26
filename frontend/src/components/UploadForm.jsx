import { useEffect, useMemo, useRef, useState } from "react";
import RouteTimeline from "./RouteTimeline";
import { useI18n } from "../i18n/useI18n";

function formatPercent(value) {
    return `${Math.round(Number(value || 0) * 100)}%`;
}

function formatOpenStatus(isOpen, t) {
    return isOpen ? t("route.open") : t("route.closed");
}

function formatUnescoStatus(isUnescoProtected, t) {
    return isUnescoProtected ? t("common.yes") : t("common.no");
}

function formatRouteDate(value, locale) {
    if (!value) {
        return "";
    }

    const date = new Date(value);

    if (Number.isNaN(date.getTime())) {
        return value;
    }

    return new Intl.DateTimeFormat(locale, {
        dateStyle: "medium",
        timeStyle: "short",
    }).format(date);
}

function getRouteStopCount(route) {
    return (route?.similarLocations?.length ?? 0) + 1;
}

function getFeedbackLabel(value, t) {
    if (value === "Patiko") {
        return t("upload.like");
    }

    if (value === "Nepatiko") {
        return t("upload.dislike");
    }

    return "";
}

function UploadForm({
    selectedFile,
    handleFileChange,
    handleAnalyzeImage,
    loading,
    analysisResult,
    selectedFeedback,
    feedbackLoading,
    onFeedbackSelect,
    routeHistory,
    routesLoading,
    routesError,
}) {
    const { locale, t } = useI18n();
    const analysis = analysisResult?.analysis;
    const file = analysisResult?.file;
    const similarLocations = analysisResult?.similarLocations ?? [];
    const detectedCategory = analysisResult?.detectedCategory;
    const detectedLocationId = analysisResult?.detectedLocationId;
    const [selectedCategory, setSelectedCategory] = useState("");
    const [expandedRouteId, setExpandedRouteId] = useState(null);
    const fileInputRef = useRef(null);
    const previewUrl = useMemo(
        () => (selectedFile ? URL.createObjectURL(selectedFile) : null),
        [selectedFile],
    );

    const filteredLocations = similarLocations.filter(
        (loc) => !selectedCategory || loc.objectType === selectedCategory,
    );
    const activeExpandedRouteId = routeHistory.some((route) => route.id === expandedRouteId)
        ? expandedRouteId
        : null;
    const canShowRoute =
        analysisResult?.routeGenerated !== false &&
        analysis &&
        similarLocations.length > 0;
    const canShowFeedbackButtons =
        Boolean(analysis && detectedLocationId) &&
        !selectedFeedback &&
        typeof onFeedbackSelect === "function";
    const feedbackSummary = getFeedbackLabel(selectedFeedback, t);
    const primarySimilarLocation = similarLocations[0] ?? null;

    useEffect(() => {
        if (!previewUrl) {
            return;
        }

        return () => URL.revokeObjectURL(previewUrl);
    }, [previewUrl]);

    const handleRemoveImage = () => {
        if (fileInputRef.current) {
            fileInputRef.current.value = "";
        }

        handleFileChange({ target: { files: [] } });
    };

    return (
        <div className="upload-section">
            <form onSubmit={handleAnalyzeImage} className="auth-form">
                <label className="upload-label" htmlFor="image-upload">
                    {t("upload.uploadLabel")}
                </label>
                <input
                    ref={fileInputRef}
                    id="image-upload"
                    type="file"
                    accept="image/png,image/jpeg,image/webp"
                    onChange={handleFileChange}
                    required
                />
                {selectedFile ? (
                    <p className="file-hint">
                        {t("upload.selectedFile", { name: selectedFile.name })}
                    </p>
                ) : null}
                {selectedFile ? (
                    <div className="image-preview">
                        <img src={previewUrl} alt={t("upload.previewAlt")} />
                        <button
                            type="button"
                            onClick={handleRemoveImage}
                            className="image-remove-button"
                            aria-label={t("upload.removeSelectedImage")}
                        >
                            x
                        </button>
                    </div>
                ) : null}
                <button type="submit" disabled={loading}>
                    {loading ? t("upload.analyzing") : t("upload.analyzeImage")}
                </button>
            </form>

            <div className="filter-panel">
                <h3 className="filter-title">{t("upload.filterLocations")}</h3>

                <select
                    className="filter-select"
                    value={selectedCategory}
                    onChange={(e) => setSelectedCategory(e.target.value)}
                >
                    <option value="">{t("common.all")}</option>
                    <option value="Church">{t("upload.categoryChurch")}</option>
                    <option value="Castle">{t("upload.categoryCastle")}</option>
                    <option value="Park">{t("upload.categoryPark")}</option>
                </select>

                {selectedCategory && filteredLocations.length === 0 ? (
                    <p className="filter-empty-message">
                        {t("upload.noLocationsForCategory")}
                    </p>
                ) : null}
            </div>

            {analysis && file ? (
                <section className="result-card">
                    <h2>{t("upload.analysisResult")}</h2>
                    <div className="result-grid">
                        <div className="result-item">
                            <span>{t("upload.fileName")}</span>
                            <strong>{file.originalFileName}</strong>
                        </div>
                        <div className="result-item">
                            <span>{t("upload.mimeType")}</span>
                            <strong>{file.mimeType}</strong>
                        </div>
                        <div className="result-item">
                            <span>{t("upload.fileSize")}</span>
                            <strong>{file.size} {t("upload.bytes")}</strong>
                        </div>
                        <div className="result-item">
                            <span>{t("upload.objectType")}</span>
                            <strong>{analysis.objectType}</strong>
                        </div>
                        <div className="result-item">
                            <span>{t("upload.architectureStyle")}</span>
                            <strong>{analysis.architectureStyle}</strong>
                        </div>
                        <div className="result-item">
                            <span>{t("upload.period")}</span>
                            <strong>{analysis.period}</strong>
                        </div>
                        <div className="result-item">
                            <span>{t("upload.city")}</span>
                            <strong>{analysis.city}</strong>
                        </div>
                        <div className="result-item">
                            <span>{t("upload.confidence")}</span>
                            <strong>{formatPercent(analysis.confidence)}</strong>
                        </div>
                        {detectedCategory ? (
                            <div className="result-item">
                                <span>{t("upload.detectedCategory")}</span>
                                <strong>{detectedCategory}</strong>
                            </div>
                        ) : null}
                        {primarySimilarLocation ? (
                            <div className="result-item result-item-status">
                                <span>{t("upload.status")}</span>
                                <button
                                    type="button"
                                    className={`status-button ${
                                        primarySimilarLocation.isOpen
                                            ? "status-button-open"
                                            : "status-button-closed"
                                    }`}
                                >
                                    {formatOpenStatus(primarySimilarLocation.isOpen, t)}
                                </button>
                                <small>
                                    {t("upload.basedOnFirstSimilarRouteLabel")}{" "}
                                    <strong>{primarySimilarLocation.name}</strong>
                                </small>
                            </div>
                        ) : null}
                    </div>

                    {canShowFeedbackButtons ? (
                        <div className="feedback-panel">
                            <div className="feedback-copy">
                                <h3>{t("upload.feedbackTitle")}</h3>
                                <p>
                                    {t("upload.feedbackDescriptionStart")}{" "}
                                    <strong>{t("upload.like")}</strong>{" "}
                                    {t("upload.feedbackDescriptionMiddle")}{" "}
                                    <strong>{t("upload.dislike")}</strong>.{" "}
                                    {t("upload.feedbackDescriptionEnd")}
                                </p>
                            </div>
                            <div className="feedback-actions">
                                <button
                                    type="button"
                                    className="feedback-button"
                                    onClick={() => onFeedbackSelect("Patiko")}
                                    disabled={feedbackLoading}
                                >
                                    {t("upload.like")}
                                </button>
                                <button
                                    type="button"
                                    className="feedback-button feedback-button-alt"
                                    onClick={() => onFeedbackSelect("Nepatiko")}
                                    disabled={feedbackLoading}
                                >
                                    {t("upload.dislike")}
                                </button>
                            </div>
                        </div>
                    ) : null}

                    {feedbackSummary ? (
                        <div className="feedback-summary">
                            <span className="feedback-summary-badge">{feedbackSummary}</span>
                            <p>
                                {t("upload.feedbackThanks")}
                            </p>
                        </div>
                    ) : null}
                </section>
            ) : null}

            {canShowRoute ? (
                <section className="result-card route-card">
                    <div className="route-header">
                        <div>
                            <h2>{t("route.similarPlacesRoute")}</h2>
                            <p className="route-subtitle">
                                {t("route.similarPlacesRouteSubtitle")}
                            </p>
                        </div>
                    </div>

                    <div className="route-timeline">
                        <article className="route-stop route-stop-start">
                            <div className="route-marker">{t("route.start")}</div>
                            <div className="route-content">
                                <div className="route-topline">
                                    <strong>{analysis.name}</strong>
                                    <span className="similarity-badge similarity-badge-primary">
                                        {formatPercent(analysis.confidence)} {t("route.confidence")}
                                    </span>
                                </div>
                                <p>{t("route.objectInCity", {
                                    objectType: analysis.objectType,
                                    city: analysis.city,
                                })}</p>
                                <small>
                                    {analysis.architectureStyle} | {analysis.period}
                                </small>
                            </div>
                        </article>

                        {filteredLocations.map((location, index) => (
                            <article key={location.id} className="route-stop">
                                <div className="route-marker">{index + 1}</div>
                                <div className="route-content">
                                    <div className="route-topline">
                                        <strong>{location.name}</strong>
                                        <span className="similarity-badge">
                                            {formatPercent(location.similarity)} {t("route.match")}
                                        </span>
                                    </div>
                                    <p>{location.city}</p>
                                    <p className="location-meta">
                                        {location.objectType} | {location.category} | {t("route.unesco")}:{" "}
                                        {formatUnescoStatus(location.isUnescoProtected, t)}
                                    </p>
                                    <p
                                        className={`location-status ${
                                            location.isOpen
                                                ? "location-status-open"
                                                : "location-status-closed"
                                        }`}
                                    >
                                        {formatOpenStatus(location.isOpen, t)}
                                    </p>
                                    <small>
                                        {location.architectureStyle} | {location.period}
                                    </small>
                                    <div className="route-progress">
                                        <div
                                            className="route-progress-bar"
                                            style={{
                                                width: `${Math.max(
                                                    6,
                                                    Math.round(
                                                        Number(location.similarity || 0) * 100,
                                                    ),
                                                )}%`,
                                            }}
                                        />
                                    </div>
                                </div>
                            </article>
                        ))}
                    </div>
                </section>
            ) : null}

            <section className="result-card route-history-section">
                <div className="route-header">
                    <div>
                        <h2>{t("route.generatedRoutes")}</h2>
                        <p className="route-subtitle">
                            {t("route.generatedRoutesSubtitle")}
                        </p>
                    </div>
                </div>

                {routesLoading ? (
                    <p className="muted-message">{t("route.loadingRoutes")}</p>
                ) : null}

                {!routesLoading && routesError ? (
                    <p className="muted-message">{routesError}</p>
                ) : null}

                {!routesLoading && !routesError && routeHistory.length === 0 ? (
                    <p className="muted-message">
                        {t("route.noGeneratedRoutes")}
                    </p>
                ) : null}

                {!routesLoading && routeHistory.length > 0 ? (
                    <div className="route-history-list">
                        {routeHistory.map((route) => {
                            const isExpanded = activeExpandedRouteId === route.id;

                            return (
                                <article key={route.id} className="route-history-item">
                                    <button
                                        type="button"
                                        className="route-history-toggle"
                                        onClick={() =>
                                            setExpandedRouteId((currentId) =>
                                                currentId === route.id ? null : route.id,
                                            )
                                        }
                                        aria-expanded={isExpanded}
                                    >
                                        <div className="route-history-main">
                                            <div className="route-history-title-row">
                                                <strong>
                                                    {route.analysis?.name || t("common.savedRoute")}
                                                </strong>
                                                <span className="route-history-city">
                                                    {route.analysis?.city || t("common.unknownCity")}
                                                </span>
                                            </div>
                                            <p className="route-history-meta">
                                                {formatRouteDate(route.createdAtUtc, locale)}
                                            </p>
                                            <div className="route-history-tags">
                                                <span className="route-history-tag">
                                                    {route.file?.originalFileName || t("common.image")}
                                                </span>
                                                <span className="route-history-tag">
                                                    {t("route.stopsCount", {
                                                        count: getRouteStopCount(route),
                                                    })}
                                                </span>
                                                <span className="route-history-tag">
                                                    {route.analysis?.architectureStyle ||
                                                        t("common.unknownStyle")}
                                                </span>
                                            </div>
                                        </div>

                                        <span className="route-history-action">
                                            <span>
                                                {isExpanded
                                                    ? t("route.hideRoute")
                                                    : t("route.showRoute")}
                                            </span>
                                            <span
                                                className={`route-history-chevron${
                                                    isExpanded ? " is-open" : ""
                                                }`}
                                                aria-hidden="true"
                                            >
                                                v
                                            </span>
                                        </span>
                                    </button>

                                    {isExpanded ? (
                                        <RouteTimeline
                                            as="div"
                                            className="route-history-panel route-card"
                                            title={route.analysis?.name || t("common.savedRoute")}
                                            subtitle={t("route.savedRouteSubtitle")}
                                            analysis={route.analysis}
                                            similarLocations={route.similarLocations}
                                        />
                                    ) : null}
                                </article>
                            );
                        })}
                    </div>
                ) : null}
            </section>
        </div>
    );
}

export default UploadForm;
