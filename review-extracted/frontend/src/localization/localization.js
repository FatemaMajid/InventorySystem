import ar from './ar';
import en from './en';

const translations = {
  ar,
  en,
};

export function getTranslations(language) {
  return translations[language] ?? translations.en;
}

export default translations;