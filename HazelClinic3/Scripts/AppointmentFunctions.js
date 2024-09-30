$(document).ready(function () {

    var consultOnlyRadio = $("#ConsultationType_ConsultOnly");
    var vaccinationCheckboxes = $(".vaccine-checkbox");

    updateCheckboxState();

    $("input[name='ConsultationType']").change(function () {
        updateCheckboxState();
    });

    function updateCheckboxState() {
        var selectedValue = $("input[name='ConsultationType']:checked").val();

        if (selectedValue === "Consult Only" || !selectedValue) {
            vaccinationCheckboxes.prop("disabled", true);
        } else {
            vaccinationCheckboxes.prop("disabled", false);
        }
    }
});

$(document).ready(function () {

    $("input[type='submit']").click(function (e) {
        var consultationType = $("input[name='ConsultationType']:checked").val();

        if (consultationType === "Vaccination and Consult") {
            if ($(".vaccine-checkbox:checked").length === 0) {
                e.preventDefault();
                $("#vaccine-error").text("Please select at least one vaccine type.");
            }
        }
    });
});

$(document).ready(function () {
    function updateCheckboxLabelsAndValues() {
        var selectedAnimalType = $("input[name='AnimalType']:checked").val();

        if (selectedAnimalType === "Domestic Animal") {
            $("#CoronaLabel").text("Corona");
            $("#DPVLabel").text("DPV");
            $("#RabiesLabel").text("Rabies");
            $("#CoronaCheckbox").val("Corona");
            $("#DPVCheckbox").val("DPV");
            $("#RabiesCheckbox").val("Rabies");
        } else if (selectedAnimalType === "Farm Animal") {
            $("#CoronaLabel").text("Clostridial");
            $("#DPVLabel").text("Leptospirosis");
            $("#RabiesLabel").text("Brucellosis");
            $("#CoronaCheckbox").val("Clostridial");
            $("#DPVCheckbox").val("Leptospirosis");
            $("#RabiesCheckbox").val("Brucellosis");
        }
    }

    updateCheckboxLabelsAndValues();

    $("input[name='AnimalType']").change(function () {
        updateCheckboxLabelsAndValues();
    });
});

$(document).ready(function () {
    var petSpeciesDropdown = $("#petSpeciesDropdown");

    var domesticAnimalOptions = [
        { Text: "Dog", Value: "Dog" },
        { Text: "Cat", Value: "Cat" },
        { Text: "Rabbit", Value: "Rabbit" }
    ];

    var farmAnimalOptions = [
        { Text: "Sheep", Value: "Sheep" },
        { Text: "Cow", Value: "Cow" },
        { Text: "Pig", Value: "Pig" }
    ];

    function updatePetSpeciesDropdown(options) {
        petSpeciesDropdown.empty();
        $.each(options, function (index, option) {
            petSpeciesDropdown.append($('<option>', {
                value: option.Value,
                text: option.Text
            }));
        });
    }

    $("input[name='AnimalType']").change(function () {
        var selectedValue = $(this).val();
        if (selectedValue === "Domestic Animal") {
            updatePetSpeciesDropdown(domesticAnimalOptions);
        } else if (selectedValue === "Farm Animal") {
            updatePetSpeciesDropdown(farmAnimalOptions);
        } else {
            petSpeciesDropdown.empty();
        }
    });

    $("input[name='AnimalType']").change(function () {
        var animalType = $("input[name='AnimalType']:checked").val();
        var clinicVisitRadio = $("#ClinicVisitRadio");
        if (animalType === "Farm Animal") {
            clinicVisitRadio.prop("disabled", true);
        } else {
            clinicVisitRadio.prop("disabled", false);
        }
    });
});
