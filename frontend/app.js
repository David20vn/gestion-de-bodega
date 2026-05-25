// =====================================================================
//  Bodega de Café — SPA frontend
//  Consume la API REST descrita en el repositorio gestion-de-bodega.
//  Cambia API_BASE si tu backend escucha en otro puerto.
// =====================================================================

const API_BASE = "http://localhost:5089/api";

/* --------------------------------------------------------------------
   Estado global
   -------------------------------------------------------------------- */
const state = {
  token: localStorage.getItem("token") || null,
  username: localStorage.getItem("username") || null,
  currentView: null,
  loadingCount: 0,
  cache: {
    clientes: [],
    tipos: [],
    secciones: [
      // La API no expone /api/secciones; estos IDs vienen de la semilla
      // del backend (ver Program.cs). Si en tu BD los IDs cambian,
      // ajústalos aquí o expón un endpoint y reemplaza loadSecciones().
      { id: 1, nombre: "Café en grano" },
      { id: 2, nombre: "Café molido" },
      { id: 3, nombre: "Café instantáneo" },
      { id: 4, nombre: "Extracto de café" },
    ],
  },
};

/* --------------------------------------------------------------------
   DOM helpers
   -------------------------------------------------------------------- */
const $ = (sel, root = document) => root.querySelector(sel);
const $$ = (sel, root = document) => Array.from(root.querySelectorAll(sel));

function escapeHtml(value) {
  if (value === null || value === undefined) return "";
  return String(value)
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&#39;");
}

function formatDate(iso) {
  if (!iso) return "";
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return iso;
  return d.toLocaleString();
}

/* --------------------------------------------------------------------
   Loader / Toast / Modal
   -------------------------------------------------------------------- */
function showLoading(active) {
  state.loadingCount = Math.max(0, state.loadingCount + (active ? 1 : -1));
  $("#loader").classList.toggle("hidden", state.loadingCount === 0);
}

function toast(message, type = "info", ms = 3800) {
  const container = $("#toasts");
  const el = document.createElement("div");
  el.className = `toast toast-${type}`;
  el.textContent = message;
  container.appendChild(el);
  setTimeout(() => {
    el.style.opacity = "0";
    el.style.transition = "opacity 0.25s ease";
    setTimeout(() => el.remove(), 260);
  }, ms);
}

function openModal({ title, html, onSubmit }) {
  const modal = $("#modal");
  $(".modal-title", modal).textContent = title;
  $(".modal-body", modal).innerHTML = html;
  modal.classList.add("open");
  modal.setAttribute("aria-hidden", "false");

  const form = $("form", modal);
  if (form && onSubmit) {
    form.addEventListener("submit", async (e) => {
      e.preventDefault();
      const submitBtn = $('button[type="submit"]', form);
      if (submitBtn) submitBtn.disabled = true;
      try {
        const data = Object.fromEntries(new FormData(form).entries());
        await onSubmit(data, form);
        closeModal();
      } catch (err) {
        toast(err.message || "Error inesperado", "error");
      } finally {
        if (submitBtn) submitBtn.disabled = false;
      }
    });
  }
}

function closeModal() {
  const modal = $("#modal");
  modal.classList.remove("open");
  modal.setAttribute("aria-hidden", "true");
  $(".modal-body", modal).innerHTML = "";
}

/* --------------------------------------------------------------------
   API helper (agrega Bearer automáticamente)
   -------------------------------------------------------------------- */
async function api(path, { method = "GET", body, query } = {}) {
  const url = new URL(API_BASE + path);
  if (query) {
    for (const [k, v] of Object.entries(query)) {
      if (v !== undefined && v !== null && v !== "") url.searchParams.set(k, v);
    }
  }

  const headers = { Accept: "application/json" };
  if (state.token) headers["Authorization"] = `Bearer ${state.token}`;
  if (body !== undefined) headers["Content-Type"] = "application/json";

  showLoading(true);
  try {
    const res = await fetch(url.toString(), {
      method,
      headers,
      body: body !== undefined ? JSON.stringify(body) : undefined,
    });

    if (res.status === 401 && state.token) {
      logout({ silent: true });
      throw new Error("Tu sesión expiró. Inicia sesión nuevamente.");
    }

    if (res.status === 204) return null;

    const text = await res.text();
    const data = text ? safeJson(text) : null;

    if (!res.ok) {
      const msg =
        (data && (data.message || data.title || data.detail)) ||
        `Error ${res.status} ${res.statusText}`;
      throw new Error(msg);
    }
    return data;
  } catch (err) {
    if (err.name === "TypeError") {
      throw new Error(
        `No se pudo conectar a la API (${API_BASE}). Verifica que el backend esté corriendo y que tenga CORS habilitado.`
      );
    }
    throw err;
  } finally {
    showLoading(false);
  }
}

function safeJson(text) {
  try { return JSON.parse(text); } catch { return null; }
}

/* --------------------------------------------------------------------
   Auth
   -------------------------------------------------------------------- */
async function login(username, password) {
  const data = await api("/auth/login", {
    method: "POST",
    body: { username, password },
  });
  if (!data?.token) throw new Error("Respuesta de login inválida");
  state.token = data.token;
  state.username = username;
  localStorage.setItem("token", data.token);
  localStorage.setItem("username", username);
  if (data.expiration) localStorage.setItem("tokenExp", data.expiration);
}

function logout({ silent = false } = {}) {
  state.token = null;
  state.username = null;
  localStorage.removeItem("token");
  localStorage.removeItem("username");
  localStorage.removeItem("tokenExp");
  $("#app-view").classList.add("hidden");
  $("#login-view").classList.remove("hidden");
  if (!silent) toast("Sesión cerrada", "info");
}

/* --------------------------------------------------------------------
   Carga inicial de datos auxiliares (para selects)
   -------------------------------------------------------------------- */
async function loadClientesCache() {
  state.cache.clientes = await api("/clientes");
  return state.cache.clientes;
}

async function loadTiposCache() {
  state.cache.tipos = await api("/tiposcafe");
  return state.cache.tipos;
}

async function loadSeccionesCache() {
  // Como la API no expone /api/secciones, derivamos lo que podamos
  // del inventario para incluir secciones nuevas que hayas creado.
  try {
    const inv = await api("/inventario");
    const map = new Map(state.cache.secciones.map((s) => [s.id, s]));
    for (const row of inv || []) {
      if (row.seccionId && !map.has(row.seccionId)) {
        map.set(row.seccionId, {
          id: row.seccionId,
          nombre: row.seccionNombre || `Sección ${row.seccionId}`,
        });
      }
    }
    state.cache.secciones = Array.from(map.values()).sort((a, b) => a.id - b.id);
  } catch {
    // Si el inventario aún está vacío usamos los defaults
  }
  return state.cache.secciones;
}

function optionsHtml(items, { selected, valueKey = "id", labelKey = "nombre", placeholder = "— Selecciona —" } = {}) {
  const opts = [];
  if (placeholder !== null) {
    opts.push(`<option value="">${escapeHtml(placeholder)}</option>`);
  }
  for (const it of items) {
    const v = it[valueKey];
    const sel = String(v) === String(selected) ? " selected" : "";
    opts.push(`<option value="${escapeHtml(v)}"${sel}>${escapeHtml(it[labelKey])}</option>`);
  }
  return opts.join("");
}

/* --------------------------------------------------------------------
   Navegación
   -------------------------------------------------------------------- */
const VIEWS = {
  clientes: { title: "Clientes", render: renderClientes },
  tipos: { title: "Tipos de Café", render: renderTipos },
  inventario: { title: "Inventario", render: renderInventario },
  entradas: { title: "Entradas", render: renderEntradas },
  salidas: { title: "Salidas", render: renderSalidas },
};

async function showView(name) {
  if (!VIEWS[name]) name = "clientes";
  state.currentView = name;

  $$(".nav-item").forEach((b) => b.classList.toggle("active", b.dataset.view === name));
  $$(".view").forEach((v) => (v.hidden = true));
  const view = $(`#view-${name}`);
  view.hidden = false;
  $("#view-title").textContent = VIEWS[name].title;
  $(".sidebar").classList.remove("open");

  try {
    await VIEWS[name].render(view);
  } catch (err) {
    toast(err.message, "error");
    view.innerHTML = `<div class="card"><p class="error-msg">${escapeHtml(err.message)}</p></div>`;
  }
}

/* ====================================================================
   Vista: CLIENTES
   ==================================================================== */
async function renderClientes(root) {
  const clientes = await api("/clientes");
  state.cache.clientes = clientes;

  root.innerHTML = `
    <div class="card">
      <div class="card-header">
        <h3>Clientes registrados <span class="badge">${clientes.length}</span></h3>
        <button class="btn btn-primary" id="btn-new-cliente">+ Nuevo cliente</button>
      </div>
      <div class="table-wrap">
        <table>
          <thead>
            <tr>
              <th>ID</th><th>Nombre</th><th>Contacto</th><th>Teléfono</th>
              <th>Email</th><th>Registro</th><th></th>
            </tr>
          </thead>
          <tbody>
            ${
              clientes.length === 0
                ? `<tr><td colspan="7" class="empty-state">Sin clientes todavía.</td></tr>`
                : clientes
                    .map(
                      (c) => `
                <tr>
                  <td>${c.id}</td>
                  <td>${escapeHtml(c.nombre)}</td>
                  <td>${escapeHtml(c.contacto)}</td>
                  <td>${escapeHtml(c.telefono)}</td>
                  <td>${escapeHtml(c.email)}</td>
                  <td>${escapeHtml(formatDate(c.fechaRegistro))}</td>
                  <td class="row-actions">
                    <button class="btn btn-sm btn-outline" data-edit="${c.id}">Editar</button>
                    <button class="btn btn-sm btn-danger" data-del="${c.id}">Eliminar</button>
                  </td>
                </tr>`
                    )
                    .join("")
            }
          </tbody>
        </table>
      </div>
    </div>
  `;

  $("#btn-new-cliente", root).addEventListener("click", () => openClienteForm());
  $$("[data-edit]", root).forEach((b) =>
    b.addEventListener("click", () => {
      const id = Number(b.dataset.edit);
      const c = clientes.find((x) => x.id === id);
      openClienteForm(c);
    })
  );
  $$("[data-del]", root).forEach((b) =>
    b.addEventListener("click", async () => {
      const id = Number(b.dataset.del);
      const c = clientes.find((x) => x.id === id);
      if (!confirm(`¿Eliminar al cliente "${c?.nombre}"?`)) return;
      try {
        await api(`/clientes/${id}`, { method: "DELETE" });
        toast("Cliente eliminado", "success");
        showView("clientes");
      } catch (err) {
        toast(err.message, "error");
      }
    })
  );
}

function openClienteForm(cliente = null) {
  const editing = !!cliente;
  openModal({
    title: editing ? `Editar cliente #${cliente.id}` : "Nuevo cliente",
    html: `
      <form>
        <div class="form-grid">
          <label><span>Nombre *</span>
            <input name="nombre" required maxlength="120" value="${escapeHtml(cliente?.nombre || "")}" />
          </label>
          <label><span>Contacto *</span>
            <input name="contacto" required maxlength="120" value="${escapeHtml(cliente?.contacto || "")}" />
          </label>
          <label><span>Teléfono</span>
            <input name="telefono" type="tel" maxlength="40" value="${escapeHtml(cliente?.telefono || "")}" />
          </label>
          <label><span>Email</span>
            <input name="email" type="email" maxlength="120" value="${escapeHtml(cliente?.email || "")}" />
          </label>
        </div>
        <div class="form-actions">
          <button type="button" class="btn btn-ghost" data-close>Cancelar</button>
          <button type="submit" class="btn btn-primary">${editing ? "Guardar cambios" : "Crear cliente"}</button>
        </div>
      </form>
    `,
    onSubmit: async (data) => {
      if (editing) {
        await api(`/clientes/${cliente.id}`, { method: "PUT", body: data });
        toast("Cliente actualizado", "success");
      } else {
        await api("/clientes", { method: "POST", body: data });
        toast("Cliente creado", "success");
      }
      showView("clientes");
    },
  });
}

/* ====================================================================
   Vista: TIPOS DE CAFÉ
   ==================================================================== */
async function renderTipos(root) {
  const tipos = await api("/tiposcafe");
  state.cache.tipos = tipos;

  root.innerHTML = `
    <div class="card">
      <div class="card-header">
        <h3>Tipos de café <span class="badge">${tipos.length}</span></h3>
        <button class="btn btn-primary" id="btn-new-tipo">+ Nuevo tipo</button>
      </div>
      <div class="table-wrap">
        <table>
          <thead>
            <tr><th>ID</th><th>Nombre</th><th>Descripción</th><th></th></tr>
          </thead>
          <tbody>
            ${
              tipos.length === 0
                ? `<tr><td colspan="4" class="empty-state">Sin tipos de café todavía.</td></tr>`
                : tipos
                    .map(
                      (t) => `
                <tr>
                  <td>${t.id}</td>
                  <td>${escapeHtml(t.nombre)}</td>
                  <td>${escapeHtml(t.descripcion || "")}</td>
                  <td class="row-actions">
                    <button class="btn btn-sm btn-outline" data-edit="${t.id}">Editar</button>
                    <button class="btn btn-sm btn-danger" data-del="${t.id}">Eliminar</button>
                  </td>
                </tr>`
                    )
                    .join("")
            }
          </tbody>
        </table>
      </div>
    </div>
  `;

  $("#btn-new-tipo", root).addEventListener("click", () => openTipoForm());
  $$("[data-edit]", root).forEach((b) =>
    b.addEventListener("click", () => {
      const t = tipos.find((x) => x.id === Number(b.dataset.edit));
      openTipoForm(t);
    })
  );
  $$("[data-del]", root).forEach((b) =>
    b.addEventListener("click", async () => {
      const id = Number(b.dataset.del);
      const t = tipos.find((x) => x.id === id);
      if (!confirm(`¿Eliminar el tipo "${t?.nombre}"?`)) return;
      try {
        await api(`/tiposcafe/${id}`, { method: "DELETE" });
        toast("Tipo eliminado", "success");
        showView("tipos");
      } catch (err) {
        toast(err.message, "error");
      }
    })
  );
}

function openTipoForm(tipo = null) {
  const editing = !!tipo;
  openModal({
    title: editing ? `Editar tipo #${tipo.id}` : "Nuevo tipo de café",
    html: `
      <form>
        <div class="form-grid" style="grid-template-columns: 1fr;">
          <label><span>Nombre *</span>
            <input name="nombre" required maxlength="80" value="${escapeHtml(tipo?.nombre || "")}" />
          </label>
          <label><span>Descripción</span>
            <textarea name="descripcion" maxlength="500">${escapeHtml(tipo?.descripcion || "")}</textarea>
          </label>
        </div>
        <div class="form-actions">
          <button type="button" class="btn btn-ghost" data-close>Cancelar</button>
          <button type="submit" class="btn btn-primary">${editing ? "Guardar" : "Crear"}</button>
        </div>
      </form>
    `,
    onSubmit: async (data) => {
      if (editing) {
        await api(`/tiposcafe/${tipo.id}`, { method: "PUT", body: data });
        toast("Tipo actualizado", "success");
      } else {
        await api("/tiposcafe", { method: "POST", body: data });
        toast("Tipo creado", "success");
      }
      showView("tipos");
    },
  });
}

/* ====================================================================
   Vista: INVENTARIO
   ==================================================================== */
async function renderInventario(root) {
  await Promise.all([loadSeccionesCache(), loadTiposCache()]);

  root.innerHTML = `
    <div class="card">
      <div class="card-header">
        <h3>Registrar / actualizar stock</h3>
      </div>
      <form id="inv-form" class="form-grid">
        <label><span>Sección *</span>
          <select name="seccionId" required>${optionsHtml(state.cache.secciones)}</select>
        </label>
        <label><span>Tipo de café *</span>
          <select name="tipoCafeId" required>${optionsHtml(state.cache.tipos)}</select>
        </label>
        <label><span>Cantidad *</span>
          <input name="cantidad" type="number" min="0" step="1" required />
        </label>
        <div style="display:flex; align-items:end;">
          <button type="submit" class="btn btn-primary btn-block">Guardar</button>
        </div>
      </form>
      <p class="muted small" style="margin: 10px 0 0;">
        El backend reemplaza la cantidad existente para la combinación sección + tipo.
      </p>
    </div>

    <div class="card">
      <div class="card-header">
        <h3>Inventario actual</h3>
        <div class="toolbar" style="margin: 0;">
          <label style="min-width: 220px;">
            <span>Filtrar por sección</span>
            <select id="inv-filter">
              <option value="">Todas las secciones</option>
              ${optionsHtml(state.cache.secciones, { placeholder: null })}
            </select>
          </label>
        </div>
      </div>
      <div id="inv-table"></div>
    </div>
  `;

  async function reloadInv() {
    const seccionId = $("#inv-filter", root).value;
    const data = seccionId
      ? await api(`/inventario/seccion/${seccionId}`)
      : await api("/inventario");
    renderInvTable(data);
  }

  function renderInvTable(rows) {
    const total = rows.reduce((acc, r) => acc + Number(r.cantidad || 0), 0);
    $("#inv-table", root).innerHTML = `
      <div class="table-wrap">
        <table>
          <thead>
            <tr><th>ID</th><th>Sección</th><th>Tipo de café</th><th>Cantidad</th></tr>
          </thead>
          <tbody>
            ${
              rows.length === 0
                ? `<tr><td colspan="4" class="empty-state">Sin registros.</td></tr>`
                : rows
                    .map(
                      (r) => `
                <tr>
                  <td>${r.id}</td>
                  <td>${escapeHtml(r.seccionNombre)}</td>
                  <td>${escapeHtml(r.tipoCafeNombre)}</td>
                  <td><strong>${r.cantidad}</strong></td>
                </tr>`
                    )
                    .join("")
            }
          </tbody>
          ${
            rows.length > 0
              ? `<tfoot><tr><td colspan="3" style="text-align:right; font-weight:600;">Total:</td><td><strong>${total}</strong></td></tr></tfoot>`
              : ""
          }
        </table>
      </div>
    `;
  }

  $("#inv-filter", root).addEventListener("change", () => reloadInv().catch((e) => toast(e.message, "error")));

  $("#inv-form", root).addEventListener("submit", async (e) => {
    e.preventDefault();
    const form = e.currentTarget;
    const body = {
      seccionId: Number(form.seccionId.value),
      tipoCafeId: Number(form.tipoCafeId.value),
      cantidad: Number(form.cantidad.value),
    };
    try {
      await api("/inventario", { method: "POST", body });
      toast("Inventario actualizado", "success");
      form.reset();
      await reloadInv();
    } catch (err) {
      toast(err.message, "error");
    }
  });

  await reloadInv();
}

/* ====================================================================
   Movimientos: helper compartido (entradas/salidas)
   ==================================================================== */
async function renderMovimientos(root, { kind }) {
  // kind: 'entradas' | 'salidas'
  const titulo = kind === "entradas" ? "entrada" : "salida";
  const tituloPlural = kind === "entradas" ? "Entradas" : "Salidas";
  const path = `/${kind}`;

  await Promise.all([loadClientesCache(), loadTiposCache(), loadSeccionesCache()]);

  root.innerHTML = `
    <div class="card">
      <div class="card-header">
        <h3>Registrar nueva ${titulo}</h3>
      </div>
      <form id="mov-form" class="form-grid">
        <label><span>Cliente *</span>
          <select name="clienteId" required>${optionsHtml(state.cache.clientes)}</select>
        </label>
        <label><span>Sección *</span>
          <select name="seccionId" required>${optionsHtml(state.cache.secciones)}</select>
        </label>
        <label><span>Tipo de café *</span>
          <select name="tipoCafeId" required>${optionsHtml(state.cache.tipos)}</select>
        </label>
        <label><span>Cantidad *</span>
          <input name="cantidad" type="number" min="1" step="1" required />
        </label>
        <label style="grid-column: 1 / -1;"><span>Notas (opcional)</span>
          <textarea name="notas" maxlength="500"></textarea>
        </label>
        <div style="grid-column: 1 / -1; display:flex; justify-content:flex-end;">
          <button type="submit" class="btn btn-primary">Registrar ${titulo}</button>
        </div>
      </form>
    </div>

    <div class="card">
      <div class="card-header">
        <h3>Historial de ${tituloPlural.toLowerCase()}</h3>
        <button class="btn btn-ghost btn-sm" id="btn-clear-filters">Limpiar filtros</button>
      </div>
      <form id="filters" class="toolbar">
        <label><span>Cliente</span>
          <select name="clienteId">${optionsHtml(state.cache.clientes, { placeholder: "Todos" })}</select>
        </label>
        <label><span>Sección</span>
          <select name="seccionId">${optionsHtml(state.cache.secciones, { placeholder: "Todas" })}</select>
        </label>
        <label><span>Tipo de café</span>
          <select name="tipoCafeId">${optionsHtml(state.cache.tipos, { placeholder: "Todos" })}</select>
        </label>
        <label><span>Desde</span>
          <input type="date" name="desde" />
        </label>
        <label><span>Hasta</span>
          <input type="date" name="hasta" />
        </label>
        <button type="submit" class="btn btn-outline">Aplicar</button>
      </form>

      <div id="mov-table"></div>
    </div>
  `;

  async function loadList() {
    const fd = new FormData($("#filters", root));
    const query = {};
    for (const [k, v] of fd.entries()) if (v) query[k] = v;
    const data = await api(path, { query });
    renderTable(data);
  }

  function renderTable(rows) {
    $("#mov-table", root).innerHTML = `
      <div class="table-wrap">
        <table>
          <thead>
            <tr>
              <th>ID</th><th>Fecha</th><th>Cliente</th><th>Sección</th>
              <th>Tipo</th><th>Cantidad</th><th>Admin</th><th>Notas</th>
            </tr>
          </thead>
          <tbody>
            ${
              rows.length === 0
                ? `<tr><td colspan="8" class="empty-state">Sin registros con esos filtros.</td></tr>`
                : rows
                    .map(
                      (r) => `
                <tr>
                  <td>${r.id}</td>
                  <td>${escapeHtml(formatDate(r.fecha))}</td>
                  <td>${escapeHtml(r.clienteNombre)}</td>
                  <td>${escapeHtml(r.seccionNombre)}</td>
                  <td>${escapeHtml(r.tipoCafeNombre)}</td>
                  <td><strong>${r.cantidad}</strong></td>
                  <td>${escapeHtml(r.adminUsername)}</td>
                  <td>${escapeHtml(r.notas || "")}</td>
                </tr>`
                    )
                    .join("")
            }
          </tbody>
        </table>
      </div>
    `;
  }

  $("#filters", root).addEventListener("submit", (e) => {
    e.preventDefault();
    loadList().catch((err) => toast(err.message, "error"));
  });

  $("#btn-clear-filters", root).addEventListener("click", () => {
    $("#filters", root).reset();
    loadList().catch((err) => toast(err.message, "error"));
  });

  $("#mov-form", root).addEventListener("submit", async (e) => {
    e.preventDefault();
    const form = e.currentTarget;
    const body = {
      clienteId: Number(form.clienteId.value),
      seccionId: Number(form.seccionId.value),
      tipoCafeId: Number(form.tipoCafeId.value),
      cantidad: Number(form.cantidad.value),
      notas: form.notas.value || null,
    };
    try {
      await api(path, { method: "POST", body });
      toast(
        kind === "entradas"
          ? "Entrada registrada"
          : "Salida registrada",
        "success"
      );
      form.reset();
      await loadList();
    } catch (err) {
      toast(err.message, "error");
    }
  });

  await loadList();
}

async function renderEntradas(root) {
  return renderMovimientos(root, { kind: "entradas" });
}

async function renderSalidas(root) {
  return renderMovimientos(root, { kind: "salidas" });
}

/* --------------------------------------------------------------------
   Bootstrap
   -------------------------------------------------------------------- */
function setupGlobalUI() {
  $("#api-base-label").textContent = API_BASE;

  $("#login-form").addEventListener("submit", async (e) => {
    e.preventDefault();
    const fd = new FormData(e.currentTarget);
    const username = fd.get("username")?.toString().trim();
    const password = fd.get("password")?.toString();
    const errEl = $("#login-error");
    errEl.textContent = "";
    try {
      await login(username, password);
      enterApp();
    } catch (err) {
      errEl.textContent = err.message;
    }
  });

  $$(".nav-item").forEach((btn) =>
    btn.addEventListener("click", () => showView(btn.dataset.view))
  );

  $("#logout-btn").addEventListener("click", () => logout());

  $("#menu-btn").addEventListener("click", () => $(".sidebar").classList.toggle("open"));

  $("#modal").addEventListener("click", (e) => {
    if (e.target.matches("[data-close], .modal-backdrop")) closeModal();
  });
  document.addEventListener("keydown", (e) => {
    if (e.key === "Escape") closeModal();
  });
}

function enterApp() {
  $("#login-view").classList.add("hidden");
  $("#app-view").classList.remove("hidden");
  $("#user-name").textContent = state.username || "—";
  showView("clientes");
}

function init() {
  setupGlobalUI();
  if (state.token) {
    enterApp();
  } else {
    $("#app-view").classList.add("hidden");
    $("#login-view").classList.remove("hidden");
  }
}

init();
