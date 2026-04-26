import { useEffect, useMemo, useState } from "react";
import {
    DEFAULT_LANGUAGE,
    LANGUAGE_STORAGE_KEY,
    LOCALES_BY_LANGUAGE,
    translations,
} from "./translations";
import { I18nContext } from "./context";

function getStoredLanguage() {
    if (typeof window === "undefined") {
        return null;
    }

    const storedLanguage = window.localStorage.getItem(LANGUAGE_STORAGE_KEY);

    return storedLanguage && translations[storedLanguage]
        ? storedLanguage
        : null;
}

function resolveTranslation(source, key) {
    return key.split(".").reduce((current, segment) => {
        if (current && typeof current === "object" && segment in current) {
            return current[segment];
        }

        return null;
    }, source);
}

function interpolate(template, values) {
    return template.replace(/\{(\w+)\}/g, (_, token) =>
        values[token] !== undefined && values[token] !== null
            ? String(values[token])
            : `{${token}}`,
    );
}

export function I18nProvider({ children }) {
    const [language, setLanguageState] = useState(
        () => getStoredLanguage() || DEFAULT_LANGUAGE,
    );

    useEffect(() => {
        window.localStorage.setItem(LANGUAGE_STORAGE_KEY, language);
        document.documentElement.lang = language;
    }, [language]);

    const value = useMemo(() => {
        const dictionary = translations[language] || translations[DEFAULT_LANGUAGE];
        const fallbackDictionary = translations[DEFAULT_LANGUAGE];

        const t = (key, values = {}) => {
            const translation =
                resolveTranslation(dictionary, key) ??
                resolveTranslation(fallbackDictionary, key);

            if (typeof translation !== "string") {
                return key;
            }

            return interpolate(translation, values);
        };

        return {
            language,
            locale: LOCALES_BY_LANGUAGE[language] || LOCALES_BY_LANGUAGE[DEFAULT_LANGUAGE],
            setLanguage: (nextLanguage) => {
                if (translations[nextLanguage]) {
                    setLanguageState(nextLanguage);
                }
            },
            t,
        };
    }, [language]);

    return <I18nContext.Provider value={value}>{children}</I18nContext.Provider>;
}
