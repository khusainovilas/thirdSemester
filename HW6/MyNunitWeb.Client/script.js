const API_BASE = 'http://localhost:5091/api/test';

document.addEventListener('DOMContentLoaded', () => {

    const form = document.getElementById('uploadForm');
    if (!form) {
        return;
    }

    form.addEventListener('submit', async (e) => {
        e.preventDefault();

        const formData = new FormData(form);

        try {
            const res = await fetch(`${API_BASE}/run`, {
                method: 'POST',
                body: formData
            });

            if (res.ok) {
                const run = await res.json();

                document.getElementById('currentSummary').textContent = 
                    `${new Date(run.timestamp).toLocaleString()} — ${run.assemblyNames}`;

                fillTable('currentTable', run.results);
                document.getElementById('currentResults').style.display = 'block';

                loadHistory();
                form.reset();
            } else {
                const errorText = await res.text();
                console.error('Server error:', res.status, errorText);
                alert('Failed to run tests: ' + res.status);
            }
        } catch (err) {
            console.error('Network error:', err);
            alert('Unable to connect to the server. Make sure dotnet run is running.');
        }
    });

    loadHistory();
});

async function loadHistory() {
    try {
        const res = await fetch(`${API_BASE}/history`);
        if (!res.ok) throw new Error('Failed to load history');
        const runs = await res.json();

        const list = document.getElementById('historyList');
        list.innerHTML = '';

        runs.forEach(run => {
            const li = document.createElement('li');
            li.innerHTML = `
                <strong>${new Date(run.timestamp).toLocaleString()}</strong><br>
                Assemblies: ${run.assemblyNames}<br>
                <small>Passed: ${run.passed} | Failed: ${run.failed} | Ignored: ${run.ignored}</small>
            `;
            li.onclick = () => showDetails(run.id);
            list.appendChild(li);
        });
    } catch (err) {
        console.error('Error loading history:', err);
    }
}

async function showDetails(id) {
    try {
        const res = await fetch(`${API_BASE}/run/${id}`);
        const run = await res.json();

        document.getElementById('detailsSummary').textContent = 
            `${new Date(run.timestamp).toLocaleString()} — ${run.assemblyNames}`;

        fillTable('detailsTable', run.results);
        document.getElementById('details').style.display = 'block';
    } catch (err) {
        console.error('Error:', err);
    }
}

function fillTable(tableId, results) {
    const tbody = document.querySelector(`#${tableId} tbody`);
    tbody.innerHTML = '';

    results.forEach(r => {
        const tr = document.createElement('tr');
        
        let time;
        if (r.status === 'Ignored') {
            time = 0;
        } else {
            time = r.duration < 0.01 ? '<0.01' : r.duration.toFixed(2);
        }

        tr.innerHTML = `
            <td>${r.testName.split(".").pop()}</td>
            <td class="${r.status.toLowerCase()}">${r.status}</td>
            <td>${time}</td>
            <td>${r.message || ''}</td>
        `;
        tbody.appendChild(tr);
    });
}