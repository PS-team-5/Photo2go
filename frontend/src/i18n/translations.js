export const DEFAULT_LANGUAGE = "lt";
export const LANGUAGE_STORAGE_KEY = "photo2go-language";

export const LOCALES_BY_LANGUAGE = {
    lt: "lt-LT",
    en: "en-US",
    ru: "ru-RU",
};

export const LANGUAGE_OPTIONS = [
    { value: "lt", label: "LT" },
    { value: "en", label: "EN" },
    { value: "ru", label: "RU" },
];

export const translations = {
    lt: {
        common: {
            brand: "Photo2Go",
            login: "Prisijungti",
            register: "Registruotis",
            username: "Vartotojo vardas",
            email: "El. paštas",
            password: "Slaptažodis",
            logout: "Atsijungti",
            loggedInAs: "Prisijungęs kaip {username}",
            loggedInAsLabel: "Prisijungęs kaip",
            language: "Kalba",
            theme: "Tema",
            light: "Šviesi",
            dark: "Tamsi",
            switchTheme: "Perjungti į {theme} režimą",
            close: "Uždaryti",
            yes: "Taip",
            no: "Ne",
            all: "Visos",
            image: "Nuotrauka",
            savedRoute: "Išsaugotas maršrutas",
            unknownCity: "Nežinomas miestas",
            unknownStyle: "Nežinomas stilius",
        },
        auth: {
            subtitle: "Vartotojo autentifikacija",
            loginLoading: "Jungiamasi...",
            registerLoading: "Registruojama...",
        },
        messages: {
            routeOnlyForVilnius:
                "Maršrutas generuojamas tik Vilniaus objektams. Objektas neatpažintas.",
            routeNotFound:
                "Nepavyko sugeneruoti maršruto, nes duomenų bazėje nerasta panašių vietų.",
            loadRoutesFailed: "Nepavyko įkelti jūsų išsaugotų maršrutų.",
            loadRoutesUnavailable:
                "Nepavyko prisijungti prie serverio, kad būtų įkelti jūsų maršrutai.",
            loginFailed: "Prisijungti nepavyko.",
            loginSuccess:
                "Prisijungimas sėkmingas. Dabar galite įkelti nuotrauką.",
            registerFailed: "Registracija nepavyko.",
            registerSuccess:
                "Registracija sėkminga. Prašome prisijungti.",
            backendUnavailable: "Nepavyko prisijungti prie serverio.",
            chooseImageFirst: "Pirmiausia pasirinkite nuotrauką.",
            imageAnalysisFailedHttp:
                "Nuotraukos analizė nepavyko (HTTP {status}).",
            routeGenerationTimeout:
                "Nepavyko sugeneruoti maršruto per 15 sekundžių. Bandykite dar kartą.",
            imageAnalysisSuccess: "Nuotrauka sėkmingai išanalizuota.",
            imageAnalysisUnavailable:
                "Nepavyko prisijungti prie nuotraukų analizės tarnybos.",
            feedbackLocationMissing:
                "Nepavyko rasti atpažintos vietos grįžtamajam ryšiui išsaugoti.",
            feedbackSaveFailed: "Nepavyko išsaugoti jūsų atsiliepimo.",
            feedbackSaved:
                "Atsiliepimas išsaugotas. AI pasitikėjimo rodmuo atnaujintas.",
            feedbackUnavailable:
                "Nepavyko prisijungti prie atsiliepimų tarnybos.",
            logoutSuccess: "Jūs atsijungėte.",
        },
        upload: {
            uploadLabel: "Įkelkite turistinės vietos nuotrauką",
            selectedFile: "Pasirinktas failas: {name}",
            previewAlt: "Peržiūra",
            removeSelectedImage: "Pašalinti pasirinktą nuotrauką",
            analyzeImage: "Analizuoti nuotrauką",
            analyzing: "Analizuojama...",
            filterLocations: "Filtruoti vietas",
            categoryChurch: "Bažnyčia",
            categoryCastle: "Pilis",
            categoryPark: "Parkas",
            noLocationsForCategory:
                "Pasirinktai kategorijai vietų nerasta.",
            analysisResult: "Analizės rezultatas",
            fileName: "Failo pavadinimas",
            mimeType: "MIME tipas",
            fileSize: "Failo dydis",
            bytes: "baitų",
            objectType: "Objekto tipas",
            architectureStyle: "Architektūros stilius",
            period: "Laikotarpis",
            city: "Miestas",
            confidence: "Pasitikėjimas",
            detectedCategory: "Nustatyta kategorija",
            status: "Būsena",
            basedOnFirstSimilarRoute:
                "Pagal pirmą panašų maršruto tašką: {name}",
            basedOnFirstSimilarRouteLabel:
                "Pagal pirmą panašų maršruto tašką:",
            feedbackTitle: "Ar radome tai, ko ieškojote?",
            feedbackDescriptionStart: "Paspauskite",
            feedbackDescriptionMiddle: "arba",
            feedbackDescriptionEnd:
                "Tai neprivalomas atsiliepimas apie AI atpažinimo tikslumą.",
            like: "Patiko",
            dislike: "Nepatiko",
            feedbackThanks: "Ačiū už jūsų atsiliepimą.",
        },
        route: {
            similarPlacesRoute: "Panašių vietų maršrutas",
            similarPlacesRouteSubtitle:
                "Pradėkite nuo atpažintos vietos ir tęskite per tinkamiausias Vilniaus vietas iš duomenų bazės.",
            generatedRoutes: "Mano sugeneruoti maršrutai",
            generatedRoutesSubtitle:
                "Čia rodomi tik jūsų paskyros sugeneruoti maršrutai.",
            loadingRoutes: "Įkeliami jūsų išsaugoti maršrutai...",
            noGeneratedRoutes: "Kol kas neturite sugeneruotų maršrutų.",
            start: "Startas",
            match: "atitikimas",
            confidence: "pasitikėjimas",
            objectInCity: "{objectType} mieste {city}",
            open: "Atidaryta",
            closed: "Šiuo metu uždaryta",
            unesco: "UNESCO",
            stopsCount: "{count} sustojimai",
            showRoute: "Rodyti maršrutą",
            hideRoute: "Slėpti maršrutą",
            savedRouteSubtitle:
                "Anksčiau sugeneruotas maršrutas, išsaugotas jūsų paskyroje.",
        },
    },
    en: {
        common: {
            brand: "Photo2Go",
            login: "Login",
            register: "Register",
            username: "Username",
            email: "Email",
            password: "Password",
            logout: "Logout",
            loggedInAs: "Logged in as {username}",
            loggedInAsLabel: "Logged in as",
            language: "Language",
            theme: "Theme",
            light: "Light",
            dark: "Dark",
            switchTheme: "Switch to {theme} mode",
            close: "Close",
            yes: "Yes",
            no: "No",
            all: "All",
            image: "Image",
            savedRoute: "Saved route",
            unknownCity: "Unknown city",
            unknownStyle: "Unknown style",
        },
        auth: {
            subtitle: "User authentication",
            loginLoading: "Logging in...",
            registerLoading: "Registering...",
        },
        messages: {
            routeOnlyForVilnius:
                "The route is generated only for objects in Vilnius. The object is not recognized.",
            routeNotFound:
                "Could not generate a route for this object because no similar places were found in the database.",
            loadRoutesFailed: "Could not load your saved routes.",
            loadRoutesUnavailable:
                "Could not connect to backend to load your routes.",
            loginFailed: "Login failed.",
            loginSuccess:
                "Login successful. You can upload a photo now.",
            registerFailed: "Registration failed.",
            registerSuccess:
                "Registration successful. Please log in.",
            backendUnavailable: "Could not connect to backend.",
            chooseImageFirst: "Please choose an image first.",
            imageAnalysisFailedHttp:
                "Image analysis failed (HTTP {status}).",
            routeGenerationTimeout:
                "Could not generate the route within 15 seconds. Please try again.",
            imageAnalysisSuccess: "Image analyzed successfully.",
            imageAnalysisUnavailable:
                "Could not connect to image analysis service.",
            feedbackLocationMissing:
                "The detected location could not be found for saving feedback.",
            feedbackSaveFailed: "Could not save your feedback.",
            feedbackSaved:
                "Feedback saved. The AI confidence display has been updated.",
            feedbackUnavailable:
                "Could not connect to the feedback service.",
            logoutSuccess: "You have been logged out.",
        },
        upload: {
            uploadLabel: "Upload a tourist place photo",
            selectedFile: "Selected file: {name}",
            previewAlt: "Preview",
            removeSelectedImage: "Remove selected image",
            analyzeImage: "Analyze image",
            analyzing: "Analyzing...",
            filterLocations: "Filter locations",
            categoryChurch: "Church",
            categoryCastle: "Castle",
            categoryPark: "Park",
            noLocationsForCategory:
                "No locations found for selected category.",
            analysisResult: "Analysis result",
            fileName: "File name",
            mimeType: "MIME type",
            fileSize: "File size",
            bytes: "bytes",
            objectType: "Object type",
            architectureStyle: "Architecture style",
            period: "Period",
            city: "City",
            confidence: "Confidence",
            detectedCategory: "Detected category",
            status: "Status",
            basedOnFirstSimilarRoute:
                "Based on first similar route: {name}",
            basedOnFirstSimilarRouteLabel: "Based on first similar route:",
            feedbackTitle: "Did we find what you were looking for?",
            feedbackDescriptionStart: "Click",
            feedbackDescriptionMiddle: "or",
            feedbackDescriptionEnd:
                "This is optional feedback about the accuracy of the AI recognition.",
            like: "Like",
            dislike: "Dislike",
            feedbackThanks: "Thank you for your feedback.",
        },
        route: {
            similarPlacesRoute: "Similar places route",
            similarPlacesRouteSubtitle:
                "Start from the detected place and continue through the most suitable Vilnius locations from the database.",
            generatedRoutes: "My generated routes",
            generatedRoutesSubtitle:
                "Only routes generated by your account are shown here.",
            loadingRoutes: "Loading your saved routes...",
            noGeneratedRoutes:
                "You do not have any generated routes yet.",
            start: "Start",
            match: "match",
            confidence: "confidence",
            objectInCity: "{objectType} in {city}",
            open: "Open",
            closed: "Currently closed",
            unesco: "UNESCO",
            stopsCount: "{count} stops",
            showRoute: "Show route",
            hideRoute: "Hide route",
            savedRouteSubtitle:
                "Previously generated route saved to your account.",
        },
    },
    ru: {
        common: {
            brand: "Photo2Go",
            login: "Войти",
            register: "Регистрация",
            username: "Имя пользователя",
            email: "Эл. почта",
            password: "Пароль",
            logout: "Выйти",
            loggedInAs: "Вы вошли как {username}",
            loggedInAsLabel: "Вы вошли как",
            language: "Язык",
            theme: "Тема",
            light: "Светлая",
            dark: "Тёмная",
            switchTheme: "Переключить на {theme} тему",
            close: "Закрыть",
            yes: "Да",
            no: "Нет",
            all: "Все",
            image: "Изображение",
            savedRoute: "Сохранённый маршрут",
            unknownCity: "Неизвестный город",
            unknownStyle: "Неизвестный стиль",
        },
        auth: {
            subtitle: "Аутентификация пользователя",
            loginLoading: "Вход...",
            registerLoading: "Регистрация...",
        },
        messages: {
            routeOnlyForVilnius:
                "Маршрут формируется только для объектов в Вильнюсе. Объект не распознан.",
            routeNotFound:
                "Не удалось сформировать маршрут, потому что в базе данных не найдено похожих мест.",
            loadRoutesFailed: "Не удалось загрузить ваши сохранённые маршруты.",
            loadRoutesUnavailable:
                "Не удалось подключиться к серверу для загрузки ваших маршрутов.",
            loginFailed: "Не удалось выполнить вход.",
            loginSuccess:
                "Вход выполнен успешно. Теперь можно загрузить изображение.",
            registerFailed: "Регистрация не удалась.",
            registerSuccess:
                "Регистрация прошла успешно. Пожалуйста, войдите.",
            backendUnavailable: "Не удалось подключиться к серверу.",
            chooseImageFirst: "Сначала выберите изображение.",
            imageAnalysisFailedHttp:
                "Анализ изображения не удался (HTTP {status}).",
            routeGenerationTimeout:
                "Не удалось сформировать маршрут в течение 15 секунд. Попробуйте ещё раз.",
            imageAnalysisSuccess: "Изображение успешно проанализировано.",
            imageAnalysisUnavailable:
                "Не удалось подключиться к сервису анализа изображений.",
            feedbackLocationMissing:
                "Не удалось найти распознанное место для сохранения отзыва.",
            feedbackSaveFailed: "Не удалось сохранить ваш отзыв.",
            feedbackSaved:
                "Отзыв сохранён. Показатель уверенности ИИ обновлён.",
            feedbackUnavailable:
                "Не удалось подключиться к сервису отзывов.",
            logoutSuccess: "Вы вышли из системы.",
        },
        upload: {
            uploadLabel: "Загрузите фото туристического места",
            selectedFile: "Выбранный файл: {name}",
            previewAlt: "Предпросмотр",
            removeSelectedImage: "Удалить выбранное изображение",
            analyzeImage: "Анализировать изображение",
            analyzing: "Анализ...",
            filterLocations: "Фильтр мест",
            categoryChurch: "Церковь",
            categoryCastle: "Замок",
            categoryPark: "Парк",
            noLocationsForCategory:
                "Для выбранной категории места не найдены.",
            analysisResult: "Результат анализа",
            fileName: "Имя файла",
            mimeType: "Тип MIME",
            fileSize: "Размер файла",
            bytes: "байт",
            objectType: "Тип объекта",
            architectureStyle: "Архитектурный стиль",
            period: "Период",
            city: "Город",
            confidence: "Уверенность",
            detectedCategory: "Определённая категория",
            status: "Статус",
            basedOnFirstSimilarRoute:
                "На основе первой похожей точки маршрута: {name}",
            basedOnFirstSimilarRouteLabel:
                "На основе первой похожей точки маршрута:",
            feedbackTitle: "Мы нашли то, что вы искали?",
            feedbackDescriptionStart: "Нажмите",
            feedbackDescriptionMiddle: "или",
            feedbackDescriptionEnd:
                "Это необязательный отзыв о точности распознавания ИИ.",
            like: "Нравится",
            dislike: "Не нравится",
            feedbackThanks: "Спасибо за ваш отзыв.",
        },
        route: {
            similarPlacesRoute: "Маршрут похожих мест",
            similarPlacesRouteSubtitle:
                "Начните с распознанного места и продолжайте по наиболее подходящим локациям Вильнюса из базы данных.",
            generatedRoutes: "Мои сгенерированные маршруты",
            generatedRoutesSubtitle:
                "Здесь показаны только маршруты, сгенерированные вашей учётной записью.",
            loadingRoutes: "Загрузка ваших сохранённых маршрутов...",
            noGeneratedRoutes:
                "У вас пока нет сгенерированных маршрутов.",
            start: "Старт",
            match: "совпадение",
            confidence: "уверенность",
            objectInCity: "{objectType} в {city}",
            open: "Открыто",
            closed: "Сейчас закрыто",
            unesco: "UNESCO",
            stopsCount: "{count} остановок",
            showRoute: "Показать маршрут",
            hideRoute: "Скрыть маршрут",
            savedRouteSubtitle:
                "Ранее сгенерированный маршрут, сохранённый в вашей учётной записи.",
        },
    },
};
