using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace aRandomKiwi.GFM
{
    internal class PawnUIOverlay_Patch
    {

        [HarmonyPatch(typeof(PawnUIOverlay), "DrawPawnGUIOverlay")]
        public class DrawPawnGUIOverlay
        {
            [HarmonyPostfix]
            public static void Listener(Pawn ___pawn)
            {
                try
                {
                    if (!Settings.showColonistTitle)
                        return;

                    if (___pawn.RaceProps.Humanlike && ___pawn.story != null && ___pawn.story.title != "" && ___pawn.story.title != null)
                    {
                        Vector2 pos0 = GenMapUI.LabelDrawPosFor(___pawn, -0.6f);

                        Vector2 pos = GenMapUI.LabelDrawPosFor(___pawn, -0.89f);
                        pos.y = pos0.y + 12 + 2;

                        float pawnLabelNameWidth = GetPawnLabelNameWidth(___pawn.story.title, 9999f, null, GameFont.Tiny);
                        Rect bgRect = new Rect(pos.x - pawnLabelNameWidth / 2f - 4f, pos.y, pawnLabelNameWidth + 8f, 12f);

                        GUI.DrawTexture(bgRect, TexUI.GrayTextBG);

                        GUI.color = Color.yellow;
                        //GenMapUI.DrawPawnLabel(pos, ___pawn.story.title, Color.yellow);
                        Text.Font = GameFont.Tiny;
                        string pawnLabel = ___pawn.story.title;


                        Rect rect;
                        Text.Anchor = TextAnchor.UpperCenter;
                        rect = new Rect(bgRect.center.x - pawnLabelNameWidth / 2f, bgRect.y - 2f, pawnLabelNameWidth, 100f);

                        Widgets.Label(rect, pawnLabel);
                        //if (pawn.Drafted)
                        //{
                        //    Widgets.DrawLineHorizontal(bgRect.center.x - pawnLabelNameWidth / 2f, bgRect.y + 11f, pawnLabelNameWidth);
                        //}
                        GUI.color = Color.white;
                        Text.Anchor = TextAnchor.UpperLeft;
                    }

                }
                catch(Exception)
                {

                }
            }

            private static float GetPawnLabelNameWidth(string pawnLabel, float truncateToWidth, Dictionary<string, string> truncatedLabelsCache, GameFont font)
            {
                GameFont font2 = Text.Font;
                Text.Font = font;
                float num;
                if (font == GameFont.Tiny)
                {
                    num = pawnLabel.GetWidthCached();
                }
                else
                {
                    num = Text.CalcSize(pawnLabel).x;
                }
                if (System.Math.Abs(System.Math.Round((double)Prefs.UIScale) - (double)Prefs.UIScale) > 1.401298464324817E-45)
                {
                    num += 0.5f;
                }
                if (num < 20f)
                {
                    num = 20f;
                }
                Text.Font = font2;
                return num;
            }
        }
    }
}