import { useEffect, useRef, useState } from "react";
import RouteTimeline from "./RouteTimeline";

function formatPercent(value) {
    return `${Math.round(Number(value || 0) * 100)}%`;
}

function formatRouteDate(value) {
    if (!value) {
        return "";
    }

    const date = new Date(value);

    if (Number.isNaN(date.getTime())) {
        return value;
    }

    return new Intl.DateTimeFormat("lt-LT", {
        dateStyle: "medium",
        timeStyle: "short",
    }).format(date);
}

function getRouteStopCount(route) {
    return (route?.similarLocations?.length ?? 0) + 1;
}

function UploadForm({
    selectedFile,
    handleFileChange,
    handleAnalyzeImage,
    loading,
    analysisResult,
    routeHistory,
    routesLoading,
    routesError,
}) {
    const analysis = analysisResult?.analysis;
    const file = analysisResult?.file;
    const similarLocations = analysisResult?.similarLocations ?? [];
    const [previewUrl, setPreviewUrl] = useState(null);
    const [expandedRouteId, setExpandedRouteId] = useState(null);
    const fileInputRef = useRef(null);

    useEffect(() => {
        if (!selectedFile) {
            setPreviewUrl(null);
            return;
        }

        const objectUrl = URL.createObjectURL(selectedFile);
        setPreviewUrl(objectUrl);

        return () => URL.revokeObjectURL(objectUrl);
    }, [selectedFile]);

    useEffect(() => {
        if (!routeHistory.some((route) => route.id === expandedRouteId)) {
            setExpandedRouteId(null);
        }
    }, [routeHistory, expandedRouteId]);

    const handleRemoveImage = () => {
        setPreviewUrl(null);

        if (fileInputRef.current) {
            fileInputRef.current.value = "";
        }

        handleFileChange({ target: { files: [] } });
    };

    return (
        <div className="upload-section">
            <form onSubmit={handleAnalyzeImage} className="auth-form">
                <label className="upload-label" htmlFor="image-upload">
                    Upload a tourist place photo
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
                        Selected file: <strong>{selectedFile.name}</strong>
                    </p>
                ) : null}
                {selectedFile ? (
                    <div
                        className="image-preview"
                        style={{
                            position: "relative",
                            display: "inline-block",
                            width: "fit-content",
                            marginTop: "10px",
                        }}
                    >
                        <img
                            src={previewUrl}
                            alt="Preview"
                            style={{
                                display: "block",
                                maxWidth: "300px",
                                borderRadius: "8px",
                            }}
                        />

                        <button
                            type="button"
                            onClick={handleRemoveImage}
                            style={{
                                position: "absolute",
                                top: "1px",
                                right: "1px",
                                background: "transparent",
                                border: "none",
                                color: "#888",
                                fontSize: "20px",
                                cursor: "pointer",
                                fontWeight: "bold",
                                transition: "color 0.2s",
                            }}
                            onMouseEnter={(e) => (e.target.style.color = "black")}
                            onMouseLeave={(e) => (e.target.style.color = "#888")}
                        >
                            x
                        </button>
                    </div>
                ) : null}
                <button type="submit" disabled={loading}>
                    {loading ? "Analyzing..." : "Analyze image"}
                </button>
            </form>

            {analysis && file ? (
                <section className="result-card">
                    <h2>Analysis result</h2>
                    <div className="result-grid">
                        <div className="result-item">
                            <span>File name</span>
                            <strong>{file.originalFileName}</strong>
                        </div>
                        <div className="result-item">
                            <span>MIME type</span>
                            <strong>{file.mimeType}</strong>
                        </div>
                        <div className="result-item">
                            <span>File size</span>
                            <strong>{file.size} bytes</strong>
                        </div>
                        <div className="result-item">
                            <span>Object type</span>
                            <strong>{analysis.objectType}</strong>
                        </div>
                        <div className="result-item">
                            <span>Architecture style</span>
                            <strong>{analysis.architectureStyle}</strong>
                        </div>
                        <div className="result-item">
                            <span>Period</span>
                            <strong>{analysis.period}</strong>
                        </div>
                        <div className="result-item">
                            <span>City</span>
                            <strong>{analysis.city}</strong>
                        </div>
                        <div className="result-item">
                            <span>Confidence</span>
                            <strong>{formatPercent(analysis.confidence)}</strong>
                        </div>
                    </div>
                </section>
            ) : null}

            <RouteTimeline
                title="Similar places route"
                subtitle="Start from the detected place and continue through the most similar locations from the database."
                analysis={analysis}
                similarLocations={similarLocations}
            />

            <section className="result-card route-history-section">
                <div className="route-header">
                    <div>
                        <h2>My generated routes</h2>
                        <p className="route-subtitle">
                            Only routes generated by your account are shown here.
                        </p>
                    </div>
                </div>

                {routesLoading ? (
                    <p className="muted-message">Loading your saved routes...</p>
                ) : null}

                {!routesLoading && routesError ? (
                    <p className="muted-message">{routesError}</p>
                ) : null}

                {!routesLoading && !routesError && routeHistory.length === 0 ? (
                    <p className="muted-message">
                        You do not have any generated routes yet.
                    </p>
                ) : null}

                {!routesLoading && routeHistory.length > 0 ? (
                    <div className="route-history-list">
                        {routeHistory.map((route) => {
                            const isExpanded = expandedRouteId === route.id;

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
                                                    {route.analysis?.name || "Saved route"}
                                                </strong>
                                                <span className="route-history-city">
                                                    {route.analysis?.city || "Unknown city"}
                                                </span>
                                            </div>
                                            <p className="route-history-meta">
                                                {formatRouteDate(route.createdAtUtc)}
                                            </p>
                                            <div className="route-history-tags">
                                                <span className="route-history-tag">
                                                    {route.file?.originalFileName || "Image"}
                                                </span>
                                                <span className="route-history-tag">
                                                    {getRouteStopCount(route)} stops
                                                </span>
                                                <span className="route-history-tag">
                                                    {route.analysis?.architectureStyle ||
                                                        "Unknown style"}
                                                </span>
                                            </div>
                                        </div>

                                        <span className="route-history-action">
                                            <span>
                                                {isExpanded
                                                    ? "Hide route"
                                                    : "Show route"}
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
                                            title={route.analysis?.name || "Saved route"}
                                            subtitle="Previously generated route saved to your account."
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
