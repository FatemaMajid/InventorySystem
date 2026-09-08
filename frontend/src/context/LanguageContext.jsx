import { createContext, useContext, useMemo, useState } from "react";
import { getTranslations } from "../localization/localization";

const LanguageContext = createContext(null);

function getInitialLanguage() {
    const savedLanguage = localStorage.getItem("language");

    if (savedLanguage === "ar" || savedLanguage === "en") {
        return savedLanguage;
    }

    return "ar";
}

export function LanguageProvider({ children }) {
    const [language, setLanguage] = useState(getInitialLanguage);

    const changeLanguage = (newLanguage) => {
        if (newLanguage !== "ar" && newLanguage !== "en") {
            return;
        }

        localStorage.setItem("language", newLanguage);
        setLanguage(newLanguage);
    };

    const locale = "en-US";

    const value = useMemo(
        () => ({
            language,
            setLanguage: changeLanguage,
            isArabic: language === "ar",
            direction: language === "ar" ? "rtl" : "ltr",
            locale,
            translations: getTranslations(language),
            formatNumber: (value, options = {}) => {
                if (value === null || value === undefined || value === "") {
                    return "";
                }

                return new Intl.NumberFormat(locale, {
                    numberingSystem: "latn",
                    ...options
                }).format(value);
            },
            formatDateTime: (value, options = {}) => {
                if (!value) {
                    return "";
                }

                const dateValue =
                    typeof value === "string" &&
                    !value.endsWith("Z") &&
                    !/[+-]\d{2}:\d{2}$/.test(value)
                        ? `${value}Z`
                        : value;

                const date = new Date(dateValue);

                if (Number.isNaN(date.getTime())) {
                    return "";
                }

                return new Intl.DateTimeFormat(locale, {
                    numberingSystem: "latn",
                    year: "numeric",
                    month: "2-digit",
                    day: "2-digit",
                    hour: "2-digit",
                    minute: "2-digit",
                    hour12: language !== "ar",
                    ...options
                }).format(date);
            }
        }),
        [language]
    );

    return (
        <LanguageContext.Provider value={value}>
            {children}
        </LanguageContext.Provider>
    );
}

export function useLanguage() {
    const context = useContext(LanguageContext);

    if (!context) {
        throw new Error(
            "useLanguage must be used inside LanguageProvider"
        );
    }

    return context;
}