'use client';

import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import en from '../locales/en.json';

i18n
    .use(initReactI18next)
    .init({
        resources: {
            en: {
                dashboard: en.dashboard
            },
        },
        lng: 'en', // default language
        fallbackLng: 'en',
        ns: ['dashboard'],
        defaultNS: 'dashboard',
        interpolation: {
            escapeValue: false, // react already safes from xss
        },
    });

export default i18n;
