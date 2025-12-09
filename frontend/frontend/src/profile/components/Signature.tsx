import { forwardRef, useEffect, useImperativeHandle, useRef } from "react";
import type { Point, SignatureHandle } from "../props.ts";

import "../profile.css";
import { useTranslation } from "react-i18next";

const Signature = forwardRef<SignatureHandle, { className?: string; strokeWidth?: number }>(
  ({ className, strokeWidth = 2 }, ref) => {
    const canvasRef = useRef<HTMLCanvasElement | null>(null);
    const strokesRef = useRef<Point[][]>([]);
    const drawing = useRef(false);

    const { t } = useTranslation();

    const resizeCanvas = () => {
      const canvas = canvasRef.current;
      if (!canvas) return;
      const ratio = Math.max(window.devicePixelRatio || 1, 1);
      const w = canvas.clientWidth;
      const h = canvas.clientHeight;
      canvas.width = Math.floor(w * ratio);
      canvas.height = Math.floor(h * ratio);
      const ctx = canvas.getContext("2d");
      if (!ctx) return;
      ctx.scale(ratio, ratio);
      redraw();
    };

    useEffect(() => {
      resizeCanvas();
      window.addEventListener("resize", resizeCanvas);
      return () => window.removeEventListener("resize", resizeCanvas);
      // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    const getCtx = () => canvasRef.current?.getContext("2d") ?? null;

    const redraw = () => {
      const canvas = canvasRef.current;
      const ctx = getCtx();
      if (!canvas || !ctx) return;
      // clear
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      // style
      ctx.lineCap = "round";
      ctx.lineJoin = "round";
      ctx.strokeStyle = "#fff"; // dark on light; your CSS can invert
      ctx.lineWidth = strokeWidth;
      // draw strokes
      for (const stroke of strokesRef.current) {
        if (!stroke.length) continue;
        ctx.beginPath();
        ctx.moveTo(stroke[0].x, stroke[0].y);
        for (let i = 1; i < stroke.length; i++) ctx.lineTo(stroke[i].x, stroke[i].y);
        ctx.stroke();
      }
    };

    const toLocalPos = (e: PointerEvent | React.PointerEvent) => {
      const canvas = canvasRef.current!;
      const rect = canvas.getBoundingClientRect();
      return { x: e.clientX - rect.left, y: e.clientY - rect.top };
    };

    const handlePointerDown = (e: React.PointerEvent) => {
      (e.target as Element).setPointerCapture((e as any).pointerId);
      drawing.current = true;
      const p = toLocalPos(e);
      strokesRef.current.push([p]);
      redraw();
    };

    const handlePointerMove = (e: React.PointerEvent) => {
      if (!drawing.current) return;
      const p = toLocalPos(e);
      const current = strokesRef.current[strokesRef.current.length - 1];
      current.push(p);
      redraw();
    };

    const handlePointerUp = (e: React.PointerEvent) => {
      drawing.current = false;
      redraw();
    };

    const clear = () => {
      strokesRef.current = [];
      redraw();
    };

    const undo = () => {
      strokesRef.current.pop();
      redraw();
    };

    const toBlob = (type = "image/png", quality?: number) =>
      new Promise<Blob | null>((resolve) => {
        const canvas = canvasRef.current;
        if (!canvas) return resolve(null);
        canvas.toBlob((b) => resolve(b), type, quality);
      });

    useImperativeHandle(ref, () => ({
      clear,
      undo,
      toBlob,

      fromBase64: (base64: string) => {
        const canvas = canvasRef.current;
        if (!canvas) return;

        const ctx = canvas.getContext("2d");
        if (!ctx) return;

        const img = new Image();
        img.onload = () => {
          ctx.clearRect(0, 0, canvas.width, canvas.height);

          const scale = Math.min(canvas.width / img.width, canvas.height / img.height);

          const x = (canvas.width - img.width * scale) / 2;
          const y = (canvas.height - img.height * scale) / 2;

          ctx.drawImage(img, x, y, img.width * scale, img.height * scale);
        };

        img.src = `data:image/png;base64,${base64}`;
      },
    }));

    return (
      <div>
        <canvas
          ref={canvasRef}
          onPointerDown={handlePointerDown}
          onPointerMove={handlePointerMove}
          onPointerUp={handlePointerUp}
          onPointerCancel={handlePointerUp}
        />
        <div style={{ display: "flex", gap: 8, marginTop: 8 }}>
          <button type="button" onClick={clear}>
            {t("Clear")}
          </button>
          <button type="button" onClick={undo}>
            {t("Undo")}
          </button>
        </div>
      </div>
    );
  }
);

export default Signature;
