import { useState, useEffect, useRef } from "react";
function UploadForm({
    selectedFile,
    handleFileChange,
    handleAnalyzeImage,
    loading,
    analysisResult,
}) {
    const analysis = analysisResult?.analysis;
    const file = analysisResult?.file;
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
                    <div
                        className="image-preview"
                        style={{
                            position: "relative",
                            display: "inline-block",
                            width: "fit-content",
                            marginTop: "10px"
                        }}
                    >
                        <img
                            src={previewUrl}
                            alt="Preview"
                            style={{
                                display: "block",
                                maxWidth: "300px",
                                borderRadius: "8px"
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
                                transition: "color 0.2s"
                            }}
                            onMouseEnter={(e) => e.target.style.color = "black"}
                            onMouseLeave={(e) => e.target.style.color = "#888"}
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
                            <strong>{Math.round(Number(analysis.confidence || 0) * 100)}%</strong>
                        </div>
                    </div>
                </section>
            ) : null}
        </div>
    );
}

export default UploadForm;
