
    document.querySelectorAll('.assign-driver-button').forEach(function (button) {
        button.addEventListener('click', function () {
            this.style.display = 'none';
            var adoptionId = this.getAttribute('data-adoption-id');
            var deliveryDateInput = document.querySelector('input[name="deliveryDate"]');
            deliveryDateInput.disabled = true;
            var deliveryDate = deliveryDateInput.value;
            localStorage.setItem('hiddenButton_' + adoptionId, 'true');
            localStorage.setItem('deliveryDate_' + adoptionId, deliveryDate);
        });
    });

    window.onload = function () {
        var buttons = document.querySelectorAll('.assign-driver-button');
    buttons.forEach(function (button) {
            var adoptionId = button.getAttribute('data-adoption-id');
    var isButtonHidden = localStorage.getItem('hiddenButton_' + adoptionId);
    if (isButtonHidden === 'true') {
        button.style.display = 'none';
            }

    var storedDate = localStorage.getItem('deliveryDate_' + adoptionId);
    if (storedDate) {
                var deliveryDateInput = document.querySelector('input[name="deliveryDate"]');
    deliveryDateInput.value = storedDate;
    deliveryDateInput.disabled = true; 
            }
        });
    };


