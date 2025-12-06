// گرفتن همه input های مورد نظر (با id های مختلف)
const inputs = document.querySelectorAll('#userName, #telPhone, #cellPhone,#ToPrice,#FromPrice');

inputs.forEach(input => {
    input.addEventListener('keydown', function (event) {
        const allowedKeys = [
            'Backspace', 'Delete', 'ArrowLeft', 'ArrowRight', 'Tab', 'Home', 'End'
        ];

        if (
            !allowedKeys.includes(event.key) &&
            !event.key.match(/[0-9]/)
        ) {
            event.preventDefault();
        }
    });
});
