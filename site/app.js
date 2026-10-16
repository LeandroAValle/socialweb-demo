const $ = (selector) => document.querySelector(selector);
const state = { snapshot: null, visible: 8 };
const number = new Intl.NumberFormat('pt-BR');
const monthName = { '2026-04': 'Abr', '2026-05': 'Mai', '2026-06': 'Jun', '2026-07': 'Jul', '2026-08': 'Ago', '2026-09': 'Set' };

function textCell(value) {
  const cell = document.createElement('td');
  cell.textContent = value;
  return cell;
}

function selectedHouseholds() {
  const territory = $('#territory').value;
  const search = $('#search').value.trim().toUpperCase();
  const status = $('#status').value;
  return state.snapshot.households.filter(family =>
    (territory === 'Todos os territórios' || family.territory === territory) &&
    family.id.includes(search) &&
    (status === 'all' || family.registrationComplete === (status === 'complete')));
}

function renderTable() {
  const families = selectedHouseholds();
  const tbody = $('#family-rows');
  tbody.replaceChildren();
  for (const family of families.slice(0, state.visible)) {
    const row = document.createElement('tr');
    row.append(textCell(family.id), textCell(family.territory), textCell(String(family.members)), textCell(family.incomeBand));
    const status = document.createElement('td');
    const badge = document.createElement('span');
    badge.className = `pill ${family.registrationComplete ? 'complete' : 'pending'}`;
    badge.textContent = family.registrationComplete ? 'Completo' : 'Pendente';
    status.append(badge);
    row.append(status);
    const action = document.createElement('td');
    const button = document.createElement('button');
    button.className = 'detail-button';
    button.textContent = 'Ver detalhes ↗';
    button.addEventListener('click', () => showDetail(family));
    action.append(button);
    row.append(action);
    tbody.append(row);
  }
  if (families.length === 0) {
    const row = document.createElement('tr');
    const cell = textCell('Nenhuma família encontrada para os filtros selecionados.');
    cell.colSpan = 6;
    row.append(cell);
    tbody.append(row);
  }
  $('#count').textContent = `${Math.min(state.visible, families.length)} de ${families.length} famílias`;
  $('#more').hidden = state.visible >= families.length;
}

function showDetail(family) {
  const attendances = state.snapshot.attendances.filter(item => item.householdId === family.id);
  const benefits = state.snapshot.benefits.filter(item => item.householdId === family.id);
  const root = $('#detail-content');
  root.replaceChildren();
  const title = document.createElement('h2');
  title.textContent = family.id;
  const intro = document.createElement('p');
  intro.textContent = 'Registro sintético de demonstração — sem dados pessoais reais.';
  root.append(title, intro);
  const grid = document.createElement('div');
  grid.className = 'detail-grid';
  for (const [label, value] of [['Território', family.territory], ['Integrantes', String(family.members)], ['Faixa de renda', family.incomeBand], ['Cadastro', family.registrationComplete ? 'Completo' : 'Pendente']]) {
    const box = document.createElement('div');
    const caption = document.createElement('span');
    caption.textContent = label;
    const valueNode = document.createElement('strong');
    valueNode.textContent = value;
    box.append(caption, valueNode);
    grid.append(box);
  }
  root.append(grid);
  for (const [heading, records, formatter] of [
    ['Atendimentos', attendances, item => `${monthName[item.month]} · ${item.kind} · ${item.status}`],
    ['Benefícios', benefits, item => `${item.type} · ${item.status}`]
  ]) {
    const h = document.createElement('h3');
    h.textContent = `${heading} (${records.length})`;
    const list = document.createElement('ul');
    list.className = 'detail-list';
    for (const item of records) {
      const line = document.createElement('li');
      line.textContent = formatter(item);
      list.append(line);
    }
    if (!records.length) {
      const line = document.createElement('li');
      line.textContent = 'Nenhum registro no período.';
      list.append(line);
    }
    root.append(h, list);
  }
  $('#detail').showModal();
}

function renderDashboard() {
  const view = state.snapshot.views[$('#territory').value];
  const metrics = view.metrics;
  $('#households').textContent = number.format(metrics.households);
  $('#people').textContent = number.format(metrics.people);
  $('#attendances').textContent = number.format(metrics.openAttendances);
  $('#benefits').textContent = number.format(metrics.activeBenefits);
  $('#quality').textContent = `${number.format(metrics.registrationQuality)}%`;
  $('.quality-circle').style.setProperty('--quality', `${metrics.registrationQuality}%`);
  $('#complete').textContent = number.format(metrics.households - metrics.incompleteRegistrations);
  $('#incomplete').textContent = number.format(metrics.incompleteRegistrations);
  const chart = $('#chart');
  chart.replaceChildren();
  const maximum = Math.max(...view.activity.map(item => item.attendances), 1);
  for (const point of view.activity) {
    const group = document.createElement('div');
    group.className = 'bar-wrap';
    const count = document.createElement('b');
    count.textContent = number.format(point.attendances);
    const bar = document.createElement('div');
    bar.className = 'bar';
    bar.style.height = `${Math.max(5, point.attendances / maximum * 100)}px`;
    const label = document.createElement('span');
    label.textContent = monthName[point.month];
    group.append(count, bar, label);
    chart.append(group);
  }
  chart.setAttribute('aria-label', `Atendimentos mensais: ${view.activity.map(item => `${monthName[item.month]} ${item.attendances}`).join(', ')}`);
  renderTable();
}

async function init() {
  try {
    const response = await fetch('data/snapshot.json');
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    state.snapshot = await response.json();
    const picker = $('#territory');
    picker.replaceChildren();
    for (const territory of state.snapshot.territories) {
      const option = document.createElement('option');
      option.value = territory;
      option.textContent = territory;
      picker.append(option);
    }
    picker.addEventListener('change', () => { state.visible = 8; renderDashboard(); });
    $('#search').addEventListener('input', () => { state.visible = 8; renderTable(); });
    $('#status').addEventListener('change', () => { state.visible = 8; renderTable(); });
    $('#more').addEventListener('click', () => { state.visible += 8; renderTable(); });
    $('#close-detail').addEventListener('click', () => $('#detail').close());
    renderDashboard();
  } catch (error) {
    $('#family-rows').replaceChildren();
    const row = document.createElement('tr');
    const cell = textCell('Não foi possível carregar os dados da demonstração.');
    cell.colSpan = 6;
    row.append(cell);
    $('#family-rows').append(row);
    console.error(error);
  }
}

init();
