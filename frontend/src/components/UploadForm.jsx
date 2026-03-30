import { useState, useEffect, useRef } from "react";

function formatPercent(value) {
    return `${Math.round(Number(value || 0) * 100)}%`;
}

function formatOpenStatus(isOpen) {
    return isOpen ? "Atidaryta" : "\u0160iuo metu u\u017edaryta";
}

function UploadForm({
    selectedFile,
    handleFileChange,
    handleAnalyzeImage,
    loading,
    analysisResult,
}) {
    const analysis = analysisResult?.analysis;
    const file = analysisResult?.file;
    const similarLocations = analysisResult?.similarLocations ?? [];
    const primarySimilarLocation = similarLocations[0] ?? null;
    const [previewUrl, setPreviewUrl] = useState(null);
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
                    <div className="image-preview">
                        <img src={previewUrl} alt="Preview" />

                        <button
                            type="button"
                            onClick={handleRemoveImage}
                            className="image-remove-button"
                            aria-label="Remove selected image"
                        >
                            ×
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
                        {primarySimilarLocation ? (
                            <div className="result-item result-item-status">
                                <span>Status</span>
                                <button
                                    type="button"
                                    className={`status-button ${
                                        primarySimilarLocation.isOpen
                                            ? "status-button-open"
                                            : "status-button-closed"
                                    }`}
                                >
                                    {formatOpenStatus(primarySimilarLocation.isOpen)}
                                </button>
                                <small>
                                    Based on first similar route:{" "}
                                    <strong>{primarySimilarLocation.name}</strong>
                                </small>
                            </div>
                        ) : null}
                    </div>
                </section>
            ) : null}

            {analysis && similarLocations.length > 0 ? (
                <section className="result-card route-card">
                    <div className="route-header">
                        <div>
                            <h2>Similar places route</h2>
                            <p className="route-subtitle">
                                Start from the detected place and continue through the most similar locations from the database.
                            </p>
                        </div>
                    </div>

                    <div className="route-timeline">
                        <article className="route-stop route-stop-start">
                            <div className="route-marker">Start</div>
                            <div className="route-content">
                                <div className="route-topline">
                                    <strong>{analysis.name}</strong>
                                    <span className="similarity-badge similarity-badge-primary">
                                        {formatPercent(analysis.confidence)} confidence
                                    </span>
                                </div>
                                <p>
                                    {analysis.objectType} in {analysis.city}
                                </p>
                                <small>
                                    {analysis.architectureStyle} | {analysis.period}
                                </small>
                            </div>
                        </article>

                        {similarLocations.map((location, index) => (
                            <article key={location.id} className="route-stop">
                                <div className="route-marker">{index + 1}</div>
                                <div className="route-content">
                                    <div className="route-topline">
                                        <strong>{location.name}</strong>
                                        <span className="similarity-badge">
                                            {formatPercent(location.similarity)} match
                                        </span>
                                    </div>
                                    <p>{location.city}</p>
                                    <p
                                        className={`location-status ${
                                            location.isOpen
                                                ? "location-status-open"
                                                : "location-status-closed"
                                        }`}
                                    >
                                        {formatOpenStatus(location.isOpen)}
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
        </div>
    );
}

export default UploadForm;
