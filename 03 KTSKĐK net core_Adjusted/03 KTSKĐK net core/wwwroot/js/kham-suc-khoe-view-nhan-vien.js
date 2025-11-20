/**
 * KhamSucKhoe - ViewNhanVienAll JavaScript Helper
 * Usage examples for calling the ViewNhanVienAll API endpoint
 */

// ============================================================================
// 1. Basic Function to Fetch All Employees
// ============================================================================
async function getAllNhanVien() {
    try {
        const response = await fetch('/KhamSucKhoe/ViewNhanVienAll', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const result = await response.json();
        if (result.success) {
            console.log(`Found ${result.data.length} employees`);
            displayNhanVienTable(result.data);
        } else {
            alert('Error: ' + result.message);
        }
    } catch (error) {
        console.error('Error fetching employees:', error);
    }
}

// ============================================================================
// 2. Filter by Department
// ============================================================================
async function getNhanVienByDonVi(donViId) {
    try {
        const response = await fetch(`/KhamSucKhoe/ViewNhanVienAll?donViId=${donViId}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const result = await response.json();
        if (result.success) {
            console.log(`Found ${result.data.length} employees in department ${donViId}`);
            displayNhanVienTable(result.data);
        } else {
            alert('Error: ' + result.message);
        }
    } catch (error) {
        console.error('Error fetching employees by department:', error);
    }
}

// ============================================================================
// 3. Search by Name or Employee ID
// ============================================================================
async function searchNhanVien(searchTerm) {
    try {
        const response = await fetch(`/KhamSucKhoe/ViewNhanVienAll?searchTerm=${encodeURIComponent(searchTerm)}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const result = await response.json();
        if (result.success) {
            console.log(`Found ${result.data.length} employees matching '${searchTerm}'`);
            displayNhanVienTable(result.data);
        } else {
            alert('No results found for: ' + searchTerm);
        }
    } catch (error) {
        console.error('Error searching employees:', error);
    }
}

// ============================================================================
// 4. Combined Filter: Department + Search
// ============================================================================
async function getNhanVienWithFilters(donViId, searchTerm) {
    try {
        let url = '/KhamSucKhoe/ViewNhanVienAll';
        const params = new URLSearchParams();
        
        if (donViId) params.append('donViId', donViId);
        if (searchTerm) params.append('searchTerm', searchTerm);
        
        if (params.toString()) {
            url += '?' + params.toString();
        }

        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const result = await response.json();
        if (result.success) {
            console.log(result.message);
            displayNhanVienTable(result.data);
        } else {
            alert('Error: ' + result.message);
        }
    } catch (error) {
        console.error('Error fetching employees:', error);
    }
}

// ============================================================================
// 5. Display Data in HTML Table
// ============================================================================
function displayNhanVienTable(data) {
    let tableHTML = `
        <table class="table table-striped table-hover">
            <thead class="table-dark">
                <tr>
                    <th>Danh S?</th>
                    <th>H? và Tên</th>
                    <th>Email</th>
                    <th>?i?n Tho?i</th>
                    <th>??n V?</th>
                    <th>Ch?c Danh</th>
                    <th>?ã ??ng Ký KSK</th>
                    <th>Ngày Khám G?n Nh?t</th>
                </tr>
            </thead>
            <tbody>
    `;

    data.forEach(nv => {
        const daDangKyKSK = nv.daDangKyKSK ? 
            '<span class="badge bg-success">?ã ??ng ký</span>' : 
            '<span class="badge bg-danger">Ch?a ??ng ký</span>';
        
        const ngayKham = nv.ngayKhamGanNhat ? 
            new Date(nv.ngayKhamGanNhat).toLocaleDateString('vi-VN') : 
            'N/A';

        tableHTML += `
            <tr>
                <td>${nv.danhSo || 'N/A'}</td>
                <td>${nv.hoTen || 'N/A'}</td>
                <td>${nv.email || 'N/A'}</td>
                <td>${nv.mobile || nv.phone_CQ || 'N/A'}</td>
                <td>${nv.donVi || 'N/A'}</td>
                <td>${nv.chucDanh || 'N/A'}</td>
                <td>${daDangKyKSK}</td>
                <td>${ngayKham}</td>
            </tr>
        `;
    });

    tableHTML += `
            </tbody>
        </table>
    `;

    // Insert into element with id 'nhanVienTableContainer'
    const container = document.getElementById('nhanVienTableContainer');
    if (container) {
        container.innerHTML = tableHTML;
    }
}

// ============================================================================
// 6. Search with Form Submission
// ============================================================================
document.getElementById('searchForm')?.addEventListener('submit', function(e) {
    e.preventDefault();
    
    const donViId = document.getElementById('donViSelect')?.value || null;
    const searchTerm = document.getElementById('searchInput')?.value || null;
    
    getNhanVienWithFilters(donViId, searchTerm);
});

// ============================================================================
// 7. Export to CSV
// ============================================================================
function exportNhanVienToCSV(data) {
    let csvContent = "data:text/csv;charset=utf-8,";
    
    // Header
    csvContent += "Danh S?,H? và Tên,Email,?i?n Tho?i,??n V?,Ch?c Danh,?ã ??ng Ký KSK,Ngày Khám G?n Nh?t\n";
    
    // Data rows
    data.forEach(nv => {
        const row = [
            nv.danhSo || '',
            nv.hoTen || '',
            nv.email || '',
            nv.mobile || nv.phone_CQ || '',
            nv.donVi || '',
            nv.chucDanh || '',
            nv.daDangKyKSK ? '?ã ??ng ký' : 'Ch?a ??ng ký',
            nv.ngayKhamGanNhat ? new Date(nv.ngayKhamGanNhat).toLocaleDateString('vi-VN') : ''
        ];
        csvContent += row.map(cell => `"${cell}"`).join(',') + '\n';
    });
    
    // Download
    const link = document.createElement('a');
    link.setAttribute('href', encodeURI(csvContent));
    link.setAttribute('download', `nhan_vien_${new Date().toISOString().slice(0, 10)}.csv`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

// ============================================================================
// 8. Statistics Summary
// ============================================================================
function displayStatistics(data) {
    const totalNhanVien = data.length;
    const daDangKyCount = data.filter(nv => nv.daDangKyKSK).length;
    const chuaDangKyCount = totalNhanVien - daDangKyCount;
    const daDangKyPercent = ((daDangKyCount / totalNhanVien) * 100).toFixed(2);

    const statsHTML = `
        <div class="row mt-4">
            <div class="col-md-3">
                <div class="card text-center">
                    <div class="card-body">
                        <h5 class="card-title">T?ng Nhân Viên</h5>
                        <p class="card-text display-4">${totalNhanVien}</p>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card text-center bg-success text-white">
                    <div class="card-body">
                        <h5 class="card-title">?ã ??ng Ký</h5>
                        <p class="card-text display-4">${daDangKyCount}</p>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card text-center bg-danger text-white">
                    <div class="card-body">
                        <h5 class="card-title">Ch?a ??ng Ký</h5>
                        <p class="card-text display-4">${chuaDangKyCount}</p>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card text-center bg-info text-white">
                    <div class="card-body">
                        <h5 class="card-title">T? L? (%)</h5>
                        <p class="card-text display-4">${daDangKyPercent}%</p>
                    </div>
                </div>
            </div>
        </div>
    `;

    const container = document.getElementById('statisticsContainer');
    if (container) {
        container.innerHTML = statsHTML;
    }
}

// ============================================================================
// 9. Example HTML Form
// ============================================================================
/*
<div class="container mt-5">
    <h2>Danh Sách Nhân Viên</h2>
    
    <form id="searchForm" class="mb-4">
        <div class="row">
            <div class="col-md-4">
                <select id="donViSelect" class="form-select">
                    <option value="">-- Ch?n ??n V? --</option>
                    <option value="1">IT Department</option>
                    <option value="2">HR Department</option>
                    <!-- Populate dynamically -->
                </select>
            </div>
            <div class="col-md-4">
                <input type="text" id="searchInput" class="form-control" placeholder="Tìm ki?m theo tên ho?c danh s?...">
            </div>
            <div class="col-md-2">
                <button type="submit" class="btn btn-primary w-100">Tìm Ki?m</button>
            </div>
            <div class="col-md-2">
                <button type="button" class="btn btn-secondary w-100" onclick="getAllNhanVien()">Reset</button>
            </div>
        </div>
    </form>

    <div id="statisticsContainer"></div>
    
    <div id="nhanVienTableContainer"></div>
</div>
*/

// ============================================================================
// 10. Initialize on Page Load
// ============================================================================
document.addEventListener('DOMContentLoaded', function() {
    // Fetch all employees on page load
    getAllNhanVien();
});
