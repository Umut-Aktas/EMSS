// Custom JavaScript for Swagger UI
document.addEventListener('DOMContentLoaded', function() {
    // Wait for Swagger UI to load
    setTimeout(function() {
        // Find sendAt input fields and convert them to datetime-local
        const sendAtInputs = document.querySelectorAll('input[data-parameter-name="sendAt"]');
        
        sendAtInputs.forEach(function(input) {
            // Change input type to datetime-local
            input.type = 'datetime-local';
            
            // Set default value to current time + 1 hour
            const now = new Date();
            now.setHours(now.getHours() + 1);
            const defaultDateTime = now.toISOString().slice(0, 16); // Format: YYYY-MM-DDTHH:MM
            input.value = defaultDateTime;
            
            // Add placeholder
            input.placeholder = 'Select date and time';
            
            // Add some styling
            input.style.padding = '8px';
            input.style.border = '1px solid #ccc';
            input.style.borderRadius = '4px';
            input.style.fontSize = '14px';
        });
        
        // Also look for any input with name containing "sendAt"
        const allInputs = document.querySelectorAll('input');
        allInputs.forEach(function(input) {
            if (input.name && input.name.includes('sendAt')) {
                input.type = 'datetime-local';
                input.placeholder = 'Select date and time';
            }
        });
    }, 1000); // Wait 1 second for Swagger to load
}); 