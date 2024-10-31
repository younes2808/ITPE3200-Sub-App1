/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.{cshtml,html}", // Sjekker alle cshtml-filer
    "./wwwroot/css/**/*.{css,html}", // Sjekker CSS-filer
    "./wwwroot/dist/**/*.{css,html}" // Sjekker eventuelle genererte CSS-filer
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}

