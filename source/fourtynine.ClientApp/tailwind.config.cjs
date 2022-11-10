/** @type {import('tailwindcss').Config} */
const colors = require('tailwindcss/colors')

module.exports = {
  content: [
    "../fourtynine/Views/**/*.cshtml",
    "./src/**/*.{js,ts}",
  ],
  theme: {
    colors,
    extend: {},
  },
  plugins: [],
}
