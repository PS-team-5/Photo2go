import { useI18n } from "../i18n/useI18n";

function formatPercent(value) {
    return `${Math.round(Number(value || 0) * 100)}%`;
}

function RouteTimeline({
    as = "section",
    className = "result-card route-card",
    title,
    subtitle,
    analysis,
    similarLocations = [],
    headerAside = null,
}) {
    const { t } = useI18n();
    const Tag = as;

    if (!analysis || similarLocations.length === 0) {
        return null;
    }

    return (
        <Tag className={className}>
            <div className="route-header">
                <div>
                    <h2>{title}</h2>
                    {subtitle ? <p className="route-subtitle">{subtitle}</p> : null}
                </div>
                {headerAside ? (
                    <div className="route-header-aside">{headerAside}</div>
                ) : null}
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

                {similarLocations.map((location, index) => (
                    <article key={`${location.id}-${index}`} className="route-stop">
                        <div className="route-marker">{index + 1}</div>
                        <div className="route-content">
                            <div className="route-topline">
                                <strong>{location.name}</strong>
                                <span className="similarity-badge">
                                    {formatPercent(location.similarity)} {t("route.match")}
                                </span>
                            </div>
                            <p>{location.city}</p>
                            <small>
                                {location.architectureStyle} | {location.period}
                            </small>
                            <div className="route-progress">
                                <div
                                    className="route-progress-bar"
                                    style={{
                                        width: `${Math.max(
                                            6,
                                            Math.round(Number(location.similarity || 0) * 100),
                                        )}%`,
                                    }}
                                />
                            </div>
                        </div>
                    </article>
                ))}
            </div>
        </Tag>
    );
}

export default RouteTimeline;
