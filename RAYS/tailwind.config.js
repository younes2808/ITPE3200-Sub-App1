/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './**/*.{razor,html,cshtml}', // Inkluderer alle Razor og CSHTML-filer
        './wwwroot/**/*.js', // Inkluderer JavaScript-filer i wwwroot
        './styles/*.css', // Inkluderer CSS-filen for å bruke Tailwind i denne filen
    ],
    theme: {
        extend: {
            screens: {
                sm: '640px', // Small screens (mobile devices)
                md: '768px', // Medium screens (tablets)
                lg: '1024px', // Large screens (laptops)
                xl: '1280px', // Extra large screens (desktops)
                '2xl': '1536px', // 2x extra large screens
                '1650px': '1650px',
                '1350px': '1350px',
                '1250px': '1250px',
                '1150px': '1150px',
                '970px': '970px',
                '870px': '870px',
                '700px': '700px',
                '580px': '580px',
                '510px': '510px',
                '500px': '500px',
                '420px': '420px',
                '400px': '400px',
                '355px': '355px',
                '300px': '300px', // Custom breakpoints
            },
        },
    },
    plugins: [
        require('@tailwindcss/forms'), // Tailwind CSS Forms plugin
        require('@tailwindcss/aspect-ratio'), // Tailwind CSS Aspect Ratio plugin
        require('@tailwindcss/typography'), // Tailwind CSS Typography plugin
    ],
};