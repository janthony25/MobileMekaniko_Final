$(function () {
    console.log("working?");


    // Initialize both charts
    initializePieChart();
    initializeBarChart();
});


// Function to initialize the pie chart for the financial overview
function initializePieChart() {
    $.ajax({
        url: '/Dashboard/GetFinancialData',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            let ctx = document.getElementById('financialOverviewChart').getContext('2d');
            let myPieChart = new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: [
                        'Total Invoiced Amount',
                        'Total Paid Amount',
                        'Remaining Balance'
                    ],
                    datasets: [{
                        data: [data.totalInvoiceAmount, data.totalPaidAmount, data.remainingBalance],
                        backgroundColor: [
                            'rgba(255, 99, 132, 0.8)',
                            'rgba(75, 192, 192, 0.8)',
                            'rgba(255, 206, 86, 0.8)'
                        ],
                        borderColor: [
                            'rgba(255, 99, 132, 1)',
                            'rgba(75, 192, 192, 1)',
                            'rgba(255, 206, 86, 1)'
                        ],
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'top',  // Moved legend to bottom
                            labels: {
                                boxWidth: 40,
                                padding: 10  // Reduced padding for the legend
                            }
                        },
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    let label = context.label || '';
                                    if (label) {
                                        label += ': ';
                                    }
                                    if (context.parsed !== null) {
                                        label += new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(context.parsed);
                                    }
                                    return label;
                                }
                            }
                        }
                    },
                    layout: {
                        padding: {
                            top: 0  // Removed the top padding to reduce space
                        }
                    }
                }
            });
        },
        error: function (xhr, status, error) {
            console.error("An error occurred while fetching financial data: " + error);
        }
    });
}


// Function to initialize the bar chart for monthly financial overview
function initializeBarChart() {
    $.ajax({
        url: '/Dashboard/GetMonthlyFinancialData',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            let ctx = document.getElementById('monthlyFinancialOverviewChart').getContext('2d');
            let myBarChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: data.months,  // e.g., ['January', 'February', ...]
                    datasets: [{
                        label: 'Total Invoiced Amount',
                        data: data.totalInvoicedAmounts,
                        backgroundColor: 'rgba(54, 162, 235, 0.8)',
                        borderColor: 'rgba(54, 162, 235, 1)',
                        borderWidth: 1
                    },
                    {
                        label: 'Total Paid Amount',
                        data: data.totalPaidAmounts,
                        backgroundColor: 'rgba(75, 192, 192, 0.8)',
                        borderColor: 'rgba(75, 192, 192, 1)',
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    scales: {
                        y: {
                            beginAtZero: true,
                            ticks: {
                                callback: function (value) {
                                    return '$' + value.toFixed(2);  // Display value with currency format
                                }
                            }
                        }
                    }
                }
            });
        },
        error: function (xhr, status, error) {
            console.error("An error occurred while fetching monthly financial data: " + error);
        }
    });
}