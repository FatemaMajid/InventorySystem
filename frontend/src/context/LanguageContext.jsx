import { createContext, useContext, useMemo, useState } from 'react';
import { getTranslations } from '../localization/localization';

const LanguageContext = createContext(null);

function getInitialLanguage() {
  const savedLanguage = localStorage.getItem('language');

  if (savedLanguage === 'ar' || savedLanguage === 'en') {
    return savedLanguage;
  }

  return 'ar';
}

export function LanguageProvider({ children }) {
  const [language, setLanguage] = useState(getInitialLanguage);

  const changeLanguage = (newLanguage) => {
    if (newLanguage !== 'ar' && newLanguage !== 'en') {
      return;
    }

    localStorage.setItem('language', newLanguage);
    setLanguage(newLanguage);
  };

  const value = useMemo(
    () => ({
      language,
      setLanguage: changeLanguage,
      isArabic: language === 'ar',
      direction: language === 'ar' ? 'rtl' : 'ltr',
      translations: getTranslations(language),
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
      'useLanguage must be used inside LanguageProvider'
    );
  }

  return context;
}