import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { useLanguage } from "../../context/LanguageContext";
import Icon from "../../components/UI/Icon/Icon";
import styles from "./Login.module.css";

function Login() {
    const {
        translations,
        direction,
        language,
        setLanguage,
    } = useLanguage();

    const { signIn, loading } = useAuth();
    const navigate = useNavigate();
    const location = useLocation();
    const t = translations.login;

    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const from = location.state?.from?.pathname || "/";

    const handleSubmit = async (event) => {
        event.preventDefault();
        setError("");

        if (!username.trim() || !password) {
            setError(t.requiredFields);
            return;
        }

        try {
            await signIn(username.trim(), password);
            navigate(from, { replace: true });
        } catch (err) {
            setError(err?.message || t.loginError);
        }
    };

    const handleLanguageChange = () => {
        setLanguage(language === "ar" ? "en" : "ar");
    };

    return (
        <main className={styles.page} dir={direction}>
            <section className={styles.login}>
                <div className={styles.brand}>
                    <div className={styles.logo}>
                        <span>IS</span>
                    </div>

                    <h1>{t.title}</h1>

                    <p>{t.description}</p>
                </div>

                <div className={styles.panel}>
                    <div className={styles.topBar}>
                        <button
                            type="button"
                            className={styles.languageButton}
                            onClick={handleLanguageChange}
                            disabled={loading}
                            aria-label={
                                language === "ar"
                                    ? translations.common.changeLanguageToEnglish
                                    : translations.common.changeLanguageToArabic
                            }
                        >
                            <Icon
                                name="language"
                                size={18}
                            />

                            <span>
                                {language === "ar"
                                    ? translations.common.english
                                    : translations.common.arabic}
                            </span>
                        </button>
                    </div>

                    <div className={styles.content}>
                        <div className={styles.heading}>
                            <span>{t.title}</span>

                            <h2>{t.signIn}</h2>

                            <p>{t.description}</p>
                        </div>

                        <form
                            className={styles.form}
                            onSubmit={handleSubmit}
                        >
                            <div className={styles.field}>
                                <label htmlFor="username">
                                    {t.username}
                                </label>

                                <div className={styles.input}>
                                    <input
                                        id="username"
                                        type="text"
                                        value={username}
                                        onChange={(event) =>
                                            setUsername(event.target.value)
                                        }
                                        placeholder={
                                            t.usernamePlaceholder
                                        }
                                        autoComplete="username"
                                        disabled={loading}
                                        autoFocus
                                    />
                                </div>
                            </div>

                            <div className={styles.field}>
                                <label htmlFor="password">
                                    {t.password}
                                </label>

                                <div className={styles.input}>
                                    <input
                                        id="password"
                                        type="password"
                                        value={password}
                                        onChange={(event) =>
                                            setPassword(event.target.value)
                                        }
                                        placeholder={
                                            t.passwordPlaceholder
                                        }
                                        autoComplete="current-password"
                                        disabled={loading}
                                    />
                                </div>
                            </div>

                            {error && (
                                <div
                                    className={styles.error}
                                    role="alert"
                                >
                                    {error}
                                </div>
                            )}

                            <button
                                type="submit"
                                className={styles.submit}
                                disabled={loading}
                            >
                                <span>
                                    {loading
                                        ? t.signingIn
                                        : t.signIn}
                                </span>

                                <Icon
                                    name={
                                        direction === "rtl"
                                            ? "arrowLeft"
                                            : "arrowRight"
                                    }
                                    size={18}
                                />
                            </button>
                        </form>
                    </div>
                </div>
            </section>
        </main>
    );
}

export default Login;