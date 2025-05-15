// Funkcja do czyszczenia wszystkich ciasteczek
window.clearAllCookies = function () {
    // Pobierz wszystkie ciasteczka
    const cookies = document.cookie.split(';');
    
    // Dla każdego ciasteczka ustaw datę wygaśnięcia w przeszłości
    for (let i = 0; i < cookies.length; i++) {
        const cookie = cookies[i];
        const eqPos = cookie.indexOf('=');
        const name = eqPos > -1 ? cookie.substr(0, eqPos).trim() : cookie.trim();
        document.cookie = name + '=;expires=Thu, 01 Jan 1970 00:00:00 GMT;path=/';
    }
    
    console.log('Wszystkie ciasteczka zostały wyczyszczone');
};

// Funkcja do zakończenia sesji Blazor
window.forceReload = function () {
    location.reload();
};

// Funkcja do przekierowania na stronę główną
window.redirectToHome = function () {
    window.location.href = '/';
}; 