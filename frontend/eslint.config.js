import js from "@eslint/js";
import globals from "globals";

export default [
    {
        ignores: ["dist/**", "node_modules/**", "coverage/**"]
    },
    {
        files: ["**/*.{js,jsx,mjs,cjs}"],
        ...js.configs.recommended,
        languageOptions: {
            globals: {
                ...globals.browser,
                ...globals.node
            },
            parserOptions: {
                ecmaFeatures: {
                    jsx: true
                }
            }
        }
    }
];