using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KutuphaneProjesi
{
    public static class ThemeHelper
    {
        public static void TemaUygula(Control parentControl, bool siyahTemaAcikMi)
        {
            if (siyahTemaAcikMi)
            {
                parentControl.BackColor = Color.Black;

                foreach (Control kontrol in parentControl.Controls)
                {
                    if (kontrol is Label)
                    {
                        kontrol.ForeColor = Color.Gray;
                    }
                    else if (kontrol is TextBox)
                    {
                        kontrol.BackColor = Color.Gray;
                        kontrol.ForeColor = Color.White;
                    }
                    else if (kontrol is Button)
                    {
                        kontrol.BackColor = Color.DarkGray;
                        kontrol.ForeColor = Color.Black;
                    }
                    else if (kontrol is DateTimePicker)
                    {
                        var dtp = kontrol as DateTimePicker;
                        dtp.CalendarMonthBackground = Color.Gray;
                        dtp.CalendarForeColor = Color.White;
                        dtp.BackColor = Color.Gray;
                        dtp.ForeColor = Color.White;
                    }


                    if (kontrol.HasChildren)
                    {
                        TemaUygula(kontrol, siyahTemaAcikMi);
                    }
                }
            }
            else
            {
                parentControl.BackColor = SystemColors.Control;

                foreach (Control kontrol in parentControl.Controls)
                {
                    if (kontrol is Label)
                    {
                        kontrol.ForeColor = SystemColors.ControlText;
                    }
                    else if (kontrol is TextBox)
                    {
                        kontrol.BackColor = SystemColors.Window;
                        kontrol.ForeColor = SystemColors.ControlText;
                    }
                    else if (kontrol is Button)
                    {
                        kontrol.BackColor = Color.White;
                        kontrol.ForeColor = SystemColors.ControlText;
                    }
                    else if (kontrol is DateTimePicker)
                    {
                        var dtp = kontrol as DateTimePicker;
                        dtp.CalendarMonthBackground = SystemColors.Window;
                        dtp.CalendarForeColor = SystemColors.ControlText;
                        dtp.BackColor = SystemColors.Window;
                        dtp.ForeColor = SystemColors.ControlText;
                    }


                    if (kontrol.HasChildren)
                    {
                        TemaUygula(kontrol, siyahTemaAcikMi);
                    }
                }
            }
        }
    }
}
