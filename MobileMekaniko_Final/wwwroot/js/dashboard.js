$(function () {
    console.log("working?");

    // Function to initialize the chart
    function initializeChart(data) {
        let ctx = document.getElementById('financialOverviewChart').getContext('2d');
        let myChart = new Chart(ctx, {
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
                        position: 'top',
                        labels: {
                            generateLabels: function (chart) {
                                const data = chart.data;
                                if (data.labels.length && data.datasets.length) {
                                    return data.labels.map((label, i) => {
                                        const value = data.datasets[0].data[i];
                                        return {
                                            text: `${label}: $${value.toFixed(2)}`,
                                            fillStyle: data.datasets[0].backgroundColor[i],
                                            hidden: isNaN(value) || value === 0,
                                            lineCap: 'round',
                                            lineDash: [],
                                            lineDashOffset: 0,
                                            lineJoin: 'round',
                                            lineWidth: 1,
                                            strokeStyle: data.datasets[0].borderColor[i],
                                            pointStyle: 'circle',
                                            index: i
                                        };
                                    });
                                }
                                return [];
                            },
                            boxWidth: 40,
                            padding: 20
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
                        top: 50  // Add some padding at the top for the legend
                    }
                }
            }
        });
    }

    // AJAX call to fetch data
    $.ajax({
        url: '/Dashboard/GetFinancialData',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            initializeChart(response);
        },
        error: function (xhr, status, error) {
            console.error("An error occurred while fetching data: " + error);
            // Optionally, display an error message to the user
        }
    });
});