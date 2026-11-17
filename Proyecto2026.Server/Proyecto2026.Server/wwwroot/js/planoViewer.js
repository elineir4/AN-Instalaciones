const instances = new Map();
let libredwgPromise = null;

async function getLibreDwg() {
    if (!libredwgPromise) {
        libredwgPromise = (async () => {
            const mod = await import('https://cdn.jsdelivr.net/npm/@mlightcad/libredwg-web@0.9.0/dist/libredwg-web.js');
            const lib = await mod.LibreDwg.create('https://cdn.jsdelivr.net/npm/@mlightcad/libredwg-web@0.9.0/wasm');
            return { lib, Dwg_File_Type: mod.Dwg_File_Type };
        })().catch((err) => {
            libredwgPromise = null;
            throw err;
        });
    }
    return libredwgPromise;
}

function parseDxfText(text) {
    const rawLines = text.replace(/\r\n/g, '\n').replace(/\r/g, '\n').split('\n');
    const pairs = [];
    for (let i = 0; i + 1 < rawLines.length; i += 2) {
        pairs.push({
            code: parseInt(rawLines[i].trim(), 10),
            value: rawLines[i + 1].trim()
        });
    }

    const entities = [];
    let inEntities = false;
    let current = null;
    let inPolyline = false;
    let polyVertices = [];

    const flushCurrent = () => {
        if (!current) return;
        if (current.type === 'LWPOLYLINE' && current._pts.length > 0) {
            entities.push({
                type: 'POLYLINE',
                points: current._pts,
                closed: (current.flags & 1) === 1
            });
        } else if (current.type !== 'POLYLINE' && current.type !== 'VERTEX' && current.type !== 'SEQEND') {
            entities.push(current);
        }
        current = null;
    };

    for (let i = 0; i < pairs.length; i++) {
        const { code, value } = pairs[i];

        if (code === 2 && value === 'ENTITIES') {
            inEntities = true;
            continue;
        }
        if (inEntities && code === 0 && value === 'ENDSEC') {
            flushCurrent();
            break;
        }
        if (!inEntities) continue;

        if (code === 0) {
            if (value === 'VERTEX' && inPolyline) {
                if (current && current.type === 'VERTEX') {
                    polyVertices.push({ x: current.x || 0, y: current.y || 0 });
                }
                current = { type: 'VERTEX', x: 0, y: 0 };
                continue;
            }
            if (value === 'SEQEND' && inPolyline) {
                if (current && current.type === 'VERTEX') {
                    polyVertices.push({ x: current.x || 0, y: current.y || 0 });
                }
                if (polyVertices.length > 0) {
                    entities.push({ type: 'POLYLINE', points: polyVertices, closed: false });
                }
                inPolyline = false;
                polyVertices = [];
                current = null;
                continue;
            }

            flushCurrent();

            if (value === 'POLYLINE') {
                inPolyline = true;
                polyVertices = [];
                current = { type: 'POLYLINE' };
            } else if (value === 'LWPOLYLINE') {
                current = { type: 'LWPOLYLINE', _pts: [], _curX: null, flags: 0 };
            } else {
                current = { type: value };
            }
            continue;
        }

        if (!current) continue;

        if (current.type === 'LWPOLYLINE') {
            if (code === 10) {
                current._curX = parseFloat(value);
            } else if (code === 20 && current._curX !== null) {
                current._pts.push({ x: current._curX, y: parseFloat(value) });
                current._curX = null;
            } else if (code === 70) {
                current.flags = parseInt(value, 10) || 0;
            }
        } else if (current.type === 'VERTEX') {
            if (code === 10) current.x = parseFloat(value);
            else if (code === 20) current.y = parseFloat(value);
        } else {
            switch (code) {
                case 1: current.text = value; break;
                case 10: current.x1 = parseFloat(value); break;
                case 20: current.y1 = parseFloat(value); break;
                case 11: current.x2 = parseFloat(value); break;
                case 21: current.y2 = parseFloat(value); break;
                case 40: current.r = parseFloat(value); break;
                case 50: current.startAngle = parseFloat(value); break;
                case 51: current.endAngle = parseFloat(value); break;
            }
        }
    }

    return normalizeEntities(entities);
}

function normalizeEntities(rawEntities) {
    const out = [];
    for (const e of rawEntities) {
        if (!e || !e.type) continue;
        const t = e.type.toUpperCase();
        if (t === 'LINE') {
            const x1 = e.x1 ?? e.startPoint?.x;
            const y1 = e.y1 ?? e.startPoint?.y;
            const x2 = e.x2 ?? e.endPoint?.x;
            const y2 = e.y2 ?? e.endPoint?.y;
            if (isFiniteNum(x1) && isFiniteNum(y1) && isFiniteNum(x2) && isFiniteNum(y2)) {
                out.push({ type: 'LINE', x1, y1, x2, y2 });
            }
        } else if (t === 'CIRCLE') {
            const cx = e.x1 ?? e.center?.x;
            const cy = e.y1 ?? e.center?.y;
            const r = e.r ?? e.radius;
            if (isFiniteNum(cx) && isFiniteNum(cy) && isFiniteNum(r) && r > 0) {
                out.push({ type: 'CIRCLE', cx, cy, r });
            }
        } else if (t === 'ARC') {
            const cx = e.x1 ?? e.center?.x;
            const cy = e.y1 ?? e.center?.y;
            const r = e.r ?? e.radius;
            const sa = e.startAngle ?? 0;
            const ea = e.endAngle ?? 360;
            if (isFiniteNum(cx) && isFiniteNum(cy) && isFiniteNum(r) && r > 0) {
                out.push({ type: 'ARC', cx, cy, r, startAngle: sa, endAngle: ea });
            }
        } else if (t === 'POLYLINE' || t === 'LWPOLYLINE') {
            const pts = (e.points || e.vertices || [])
                .map(p => ({ x: p.x, y: p.y }))
                .filter(p => isFiniteNum(p.x) && isFiniteNum(p.y));
            if (pts.length >= 2) {
                out.push({ type: 'POLYLINE', points: pts, closed: !!(e.closed || e.flag === 1) });
            }
        } else if (t === 'TEXT' || t === 'MTEXT') {
            const x = e.x1 ?? e.startPoint?.x ?? e.insertionPoint?.x;
            const y = e.y1 ?? e.startPoint?.y ?? e.insertionPoint?.y;
            const txt = (e.text || e.contents || '').replace(/\\[A-Za-z0-9]+;?/g, '').trim();
            if (isFiniteNum(x) && isFiniteNum(y) && txt) {
                out.push({ type: 'TEXT', x, y, text: txt, h: e.r || e.textHeight || e.height || 10 });
            }
        }
    }
    return out;
}

function isFiniteNum(v) {
    return typeof v === 'number' && Number.isFinite(v);
}

function computeBounds(entities) {
    let minX = Infinity, minY = Infinity, maxX = -Infinity, maxY = -Infinity;
    const expand = (x, y) => {
        if (!isFiniteNum(x) || !isFiniteNum(y)) return;
        if (x < minX) minX = x;
        if (y < minY) minY = y;
        if (x > maxX) maxX = x;
        if (y > maxY) maxY = y;
    };

    for (const e of entities) {
        if (e.type === 'LINE') {
            expand(e.x1, e.y1);
            expand(e.x2, e.y2);
        } else if (e.type === 'CIRCLE' || e.type === 'ARC') {
            expand(e.cx - e.r, e.cy - e.r);
            expand(e.cx + e.r, e.cy + e.r);
        } else if (e.type === 'POLYLINE') {
            for (const p of e.points) expand(p.x, p.y);
        } else if (e.type === 'TEXT') {
            expand(e.x, e.y);
        }
    }

    if (!Number.isFinite(minX)) {
        return { minX: 0, minY: 0, maxX: 100, maxY: 100 };
    }
    return { minX, minY, maxX, maxY };
}

function renderCanvasScene(state) {
    const { canvas, entities, bounds, view, darkBg } = state;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    const dpr = window.devicePixelRatio || 1;
    const w = canvas.clientWidth;
    const h = canvas.clientHeight;
    if (w === 0 || h === 0) return;

    if (canvas.width !== Math.floor(w * dpr) || canvas.height !== Math.floor(h * dpr)) {
        canvas.width = Math.floor(w * dpr);
        canvas.height = Math.floor(h * dpr);
    }

    ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
    ctx.fillStyle = darkBg ? '#0b1c30' : '#ffffff';
    ctx.fillRect(0, 0, w, h);

    ctx.strokeStyle = darkBg ? 'rgba(148, 163, 184, 0.08)' : 'rgba(15, 23, 42, 0.05)';
    ctx.lineWidth = 1;
    const gridStep = 40;
    for (let x = 0; x < w; x += gridStep) {
        ctx.beginPath();
        ctx.moveTo(x, 0);
        ctx.lineTo(x, h);
        ctx.stroke();
    }
    for (let y = 0; y < h; y += gridStep) {
        ctx.beginPath();
        ctx.moveTo(0, y);
        ctx.lineTo(w, y);
        ctx.stroke();
    }

    const toScreenX = (x) => (x - bounds.cx) * view.scale + w / 2 + view.panX;
    const toScreenY = (y) => -(y - bounds.cy) * view.scale + h / 2 + view.panY;

    ctx.strokeStyle = darkBg ? '#60a5fa' : '#0f2b48';
    ctx.fillStyle = darkBg ? '#e2e8f0' : '#0f172a';
    ctx.lineWidth = 1.25;

    for (const e of entities) {
        if (e.type === 'LINE') {
            ctx.beginPath();
            ctx.moveTo(toScreenX(e.x1), toScreenY(e.y1));
            ctx.lineTo(toScreenX(e.x2), toScreenY(e.y2));
            ctx.stroke();
        } else if (e.type === 'CIRCLE') {
            ctx.beginPath();
            ctx.arc(toScreenX(e.cx), toScreenY(e.cy), Math.max(0.5, e.r * view.scale), 0, Math.PI * 2);
            ctx.stroke();
        } else if (e.type === 'ARC') {
            ctx.beginPath();
            const sa = (-e.endAngle * Math.PI) / 180;
            const ea = (-e.startAngle * Math.PI) / 180;
            ctx.arc(toScreenX(e.cx), toScreenY(e.cy), Math.max(0.5, e.r * view.scale), sa, ea);
            ctx.stroke();
        } else if (e.type === 'POLYLINE') {
            ctx.beginPath();
            e.points.forEach((p, idx) => {
                const sx = toScreenX(p.x);
                const sy = toScreenY(p.y);
                if (idx === 0) ctx.moveTo(sx, sy);
                else ctx.lineTo(sx, sy);
            });
            if (e.closed) ctx.closePath();
            ctx.stroke();
        } else if (e.type === 'TEXT') {
            const px = Math.max(9, Math.min(22, e.h * view.scale));
            if (px >= 7) {
                ctx.font = `${px}px "Hanken Grotesk", sans-serif`;
                ctx.fillText(e.text, toScreenX(e.x), toScreenY(e.y));
            }
        }
    }
}

function fitCanvasToView(state) {
    const w = state.canvas.clientWidth || 800;
    const h = state.canvas.clientHeight || 500;
    const bw = Math.max(1e-6, state.bounds.maxX - state.bounds.minX);
    const bh = Math.max(1e-6, state.bounds.maxY - state.bounds.minY);
    state.view.scale = Math.min((w * 0.86) / bw, (h * 0.86) / bh);
    state.view.panX = 0;
    state.view.panY = 0;
    renderCanvasScene(state);
}

function computeInitialSvgView(host, svgEl) {
    try {
        const hostRect = host.getBoundingClientRect();
        if (hostRect.width <= 0 || hostRect.height <= 0) {
            return { scale: 1, panX: 0, panY: 0 };
        }

        const items = svgEl.querySelectorAll('path, line, polyline, polygon, circle, ellipse, rect, text');
        const boxes = [];
        for (const el of items) {
            const r = el.getBoundingClientRect();
            if (r.width === 0 && r.height === 0) continue;
            boxes.push({
                minX: r.left,
                minY: r.top,
                maxX: r.right,
                maxY: r.bottom,
                cx: r.left + r.width / 2,
                cy: r.top + r.height / 2
            });
        }

        if (boxes.length < 4) {
            return { scale: 1, panX: 0, panY: 0 };
        }

        const cxs = boxes.map(b => b.cx).sort((a, b) => a - b);
        const cys = boxes.map(b => b.cy).sort((a, b) => a - b);
        const q1x = cxs[Math.floor(cxs.length * 0.15)];
        const q3x = cxs[Math.floor(cxs.length * 0.85)];
        const q1y = cys[Math.floor(cys.length * 0.15)];
        const q3y = cys[Math.floor(cys.length * 0.85)];
        const spanX = Math.max(4, q3x - q1x);
        const spanY = Math.max(4, q3y - q1y);

        const mainBoxes = boxes.filter(b =>
            b.cx >= q1x - spanX * 1.5 && b.cx <= q3x + spanX * 1.5 &&
            b.cy >= q1y - spanY * 1.5 && b.cy <= q3y + spanY * 1.5
        );

        const target = mainBoxes.length > 0 ? mainBoxes : boxes;
        let minX = Infinity, minY = Infinity, maxX = -Infinity, maxY = -Infinity;
        for (const b of target) {
            if (b.minX < minX) minX = b.minX;
            if (b.minY < minY) minY = b.minY;
            if (b.maxX > maxX) maxX = b.maxX;
            if (b.maxY > maxY) maxY = b.maxY;
        }

        const drawW = Math.max(10, maxX - minX);
        const drawH = Math.max(10, maxY - minY);
        const drawCx = (minX + maxX) / 2 - hostRect.left;
        const drawCy = (minY + maxY) / 2 - hostRect.top;
        const hostCx = hostRect.width / 2;
        const hostCy = hostRect.height / 2;

        const scale = Math.max(0.5, Math.min(25, Math.min((hostRect.width * 0.88) / drawW, (hostRect.height * 0.88) / drawH)));
        const panX = (hostCx - drawCx) * scale;
        const panY = (hostCy - drawCy) * scale;

        return { scale, panX, panY };
    } catch {
        return { scale: 1, panX: 0, panY: 0 };
    }
}

function applySvgTransform(state) {
    const { svgEl, view } = state;
    if (!svgEl) return;
    svgEl.style.transform = `translate(${view.panX}px, ${view.panY}px) scale(${view.scale})`;
}

export async function initPlanoViewer(hostId, url, extension) {
    disposePlanoViewer(hostId);

    const host = document.getElementById(hostId);
    if (!host) return { ok: false, message: 'No se encontró el contenedor del visor.' };
    host.innerHTML = '';

    try {
        const resp = await fetch(url, { credentials: 'include' });
        if (!resp.ok) {
            return { ok: false, message: `No se pudo obtener el plano (HTTP ${resp.status}).` };
        }

        const ext = (extension || '').toLowerCase();

        if (ext === '.dxf' || ext === 'dxf') {
            const text = await resp.text();
            const entities = parseDxfText(text);
            if (!entities || entities.length === 0) {
                return { ok: false, message: 'El archivo .DXF no contiene entidades 2D visibles.' };
            }

            const canvas = document.createElement('canvas');
            canvas.className = 'trb-viewer-canvas';
            host.appendChild(canvas);

            const b = computeBounds(entities);
            const state = {
                mode: 'canvas',
                host,
                canvas,
                entities,
                darkBg: true,
                bounds: {
                    ...b,
                    cx: (b.minX + b.maxX) / 2,
                    cy: (b.minY + b.maxY) / 2
                },
                view: { scale: 1, panX: 0, panY: 0 },
                dragging: false,
                lastX: 0,
                lastY: 0
            };

            attachInteraction(host, state, () => renderCanvasScene(state));
            instances.set(hostId, state);
            fitCanvasToView(state);
            return { ok: true, count: entities.length };
        }

        // DWG: renderizado SVG completo con bloques, cotas y texto usando LibreDWG
        const buffer = await resp.arrayBuffer();
        const bytes = new Uint8Array(buffer);
        const { lib, Dwg_File_Type } = await getLibreDwg();
        const dwg = lib.dwg_read_data(bytes, Dwg_File_Type.DWG);
        if (!dwg) {
            return { ok: false, message: 'No se pudo decodificar este archivo .DWG en el navegador.' };
        }

        const db = lib.convert(dwg);
        const svgString = lib.dwg_to_svg(db);
        const entityCount = db?.entities?.length || 0;
        try { lib.dwg_free(dwg); } catch { }

        if (!svgString || !svgString.includes('<svg')) {
            return { ok: false, message: 'No se pudo generar la vista previa SVG de este archivo .DWG.' };
        }

        host.innerHTML = svgString;
        const svgEl = host.querySelector('svg');
        if (!svgEl) {
            return { ok: false, message: 'No se pudo montar el gráfico SVG.' };
        }

        svgEl.style.width = '100%';
        svgEl.style.height = '100%';
        svgEl.style.transformOrigin = 'center center';
        svgEl.style.display = 'block';
        svgEl.setAttribute('preserveAspectRatio', 'xMidYMid meet');

        const initialView = computeInitialSvgView(host, svgEl);
        const state = {
            mode: 'svg',
            host,
            svgEl,
            darkBg: true,
            initialView,
            view: { ...initialView },
            dragging: false,
            lastX: 0,
            lastY: 0
        };

        host.style.backgroundColor = '#0b1c30';
        attachInteraction(host, state, () => applySvgTransform(state));
        instances.set(hostId, state);
        applySvgTransform(state);

        return { ok: true, count: entityCount };
    } catch (err) {
        return {
            ok: false,
            message: `No se pudo renderizar la vista previa (${err?.message || 'error desconocido'}).`
        };
    }
}

function attachInteraction(el, state, redraw) {
    const onPointerDown = (e) => {
        state.dragging = true;
        state.lastX = e.clientX;
        state.lastY = e.clientY;
        el.setPointerCapture(e.pointerId);
    };
    const onPointerMove = (e) => {
        if (!state.dragging) return;
        state.view.panX += e.clientX - state.lastX;
        state.view.panY += e.clientY - state.lastY;
        state.lastX = e.clientX;
        state.lastY = e.clientY;
        redraw();
    };
    const onPointerUp = (e) => {
        state.dragging = false;
        try { el.releasePointerCapture(e.pointerId); } catch { }
    };
    const onWheel = (e) => {
        e.preventDefault();
        const factor = e.deltaY < 0 ? 1.15 : 1 / 1.15;
        state.view.scale = Math.max(0.1, Math.min(40, state.view.scale * factor));
        redraw();
    };
    const onResize = () => redraw();

    el.addEventListener('pointerdown', onPointerDown);
    el.addEventListener('pointermove', onPointerMove);
    el.addEventListener('pointerup', onPointerUp);
    el.addEventListener('wheel', onWheel, { passive: false });
    window.addEventListener('resize', onResize);

    state.cleanup = () => {
        el.removeEventListener('pointerdown', onPointerDown);
        el.removeEventListener('pointermove', onPointerMove);
        el.removeEventListener('pointerup', onPointerUp);
        el.removeEventListener('wheel', onWheel);
        window.removeEventListener('resize', onResize);
    };
}

export function zoomPlano(hostId, factor) {
    const state = instances.get(hostId);
    if (!state) return;
    state.view.scale = Math.max(0.1, Math.min(40, state.view.scale * factor));
    if (state.mode === 'canvas') renderCanvasScene(state);
    else applySvgTransform(state);
}

export function resetPlano(hostId) {
    const state = instances.get(hostId);
    if (!state) return;
    if (state.mode === 'canvas') {
        fitCanvasToView(state);
    } else {
        state.view = state.initialView ? { ...state.initialView } : { scale: 1, panX: 0, panY: 0 };
        applySvgTransform(state);
    }
}

export function toggleFondoPlano(hostId) {
    const state = instances.get(hostId);
    if (!state) return;
    state.darkBg = !state.darkBg;
    if (state.mode === 'canvas') {
        renderCanvasScene(state);
    } else {
        state.host.style.backgroundColor = state.darkBg ? '#0b1c30' : '#ffffff';
    }
}

export function disposePlanoViewer(hostId) {
    const state = instances.get(hostId);
    if (state) {
        state.cleanup?.();
        if (state.host) state.host.innerHTML = '';
        instances.delete(hostId);
    }
}
