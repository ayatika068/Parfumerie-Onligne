// AJAX Add to Cart
function addToCart(productId) {
    fetch('/Cart/AddToCartApi', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
        },
        body: `productId=${productId}&quantity=1`
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                updateCartBadge(data.cartCount);
                showToast(data.message, 'success');
            } else {
                showToast(data.message, 'error');
            }
        })
        .catch(error => console.error('Error:', error));
}

// Dynamic Filtering
function filterProducts() {
    const categoryId = document.getElementById('categoryFilter').value;
    const maxPrice = document.getElementById('priceFilter').value;

    let url = `/Shop/Filter?maxPrice=${maxPrice}`;
    if (categoryId) {
        url += `&categoryId=${categoryId}`;
    }

    fetch(url)
        .then(response => response.text())
        .then(html => {
            document.getElementById('productsContainer').innerHTML = html;
        })
        .catch(error => console.error('Error:', error));
}

// Toast Notification
function showToast(message, type = 'success') {
    const container = document.getElementById('toast-container');
    const toast = document.createElement('div');
    toast.className = `toast show ${type}`;
    toast.style.minWidth = '250px';
    toast.style.marginBottom = '10px';
    toast.style.backgroundColor = type === 'success' ? '#28a745' : '#dc3545';
    toast.style.color = 'white';
    toast.style.padding = '15px';
    toast.style.borderRadius = '5px';
    toast.style.boxShadow = '0 4px 6px rgba(0,0,0,0.1)';
    toast.style.animation = 'slideIn 0.3s ease-out';

    toast.innerHTML = `
        <div style="display: flex; align-items: center; justify-content: space-between;">
            <span>${message}</span>
            <button onclick="this.parentElement.parentElement.remove()" style="background:none; border:none; color:white; cursor:pointer;">&times;</button>
        </div>
    `;

    container.appendChild(toast);

    setTimeout(() => {
        toast.style.animation = 'fadeOut 0.5s ease-out';
        setTimeout(() => toast.remove(), 450);
    }, 3000);
}

// Add CSS animation for toast
const style = document.createElement('style');
style.innerHTML = `
    @keyframes slideIn {
        from { transform: translateX(100%); opacity: 0; }
        to { transform: translateX(0); opacity: 1; }
    }
    @keyframes fadeOut {
        from { opacity: 1; }
        to { opacity: 0; }
    }
`;
document.head.appendChild(style);
