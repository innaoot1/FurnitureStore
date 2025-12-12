using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FurnitureStore
{
    public static class AutoLockManager
    {
        private static Timer lockTimer;
        private static int lockTimeout = 30000;
        private static DateTime lastUserAction = DateTime.Now;
        private static bool isMonitoringActive = false;

        public static void StartMonitoring()
        {
            if (lockTimer != null) return;

            lockTimer = new Timer();
            lockTimer.Interval = 1000;
            lockTimer.Tick += LockTimer_Tick;
            lockTimer.Start();

            isMonitoringActive = true;

            Application.AddMessageFilter(new UserActionTracker());
        }

        private static void LockTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan inactivePeriod = DateTime.Now - lastUserAction;

            if (inactivePeriod.TotalMilliseconds >= lockTimeout && isMonitoringActive)
            {
                TriggerLock();
            }
        }

        public static void RecordUserAction()
        {
            lastUserAction = DateTime.Now;
        }

        public static void SuspendMonitoring()
        {
            isMonitoringActive = false;
            if (lockTimer != null)
            {
                lockTimer.Stop();
            }
        }

        public static void ResumeMonitoring()
        {
            isMonitoringActive = true;
            RecordUserAction();
            if (lockTimer != null)
            {
                lockTimer.Start();
            }
        }

        private static void TriggerLock()
        {
            SuspendMonitoring();

            Form currentActiveForm = Form.ActiveForm;

            MessageBox.Show("Сеанс заблокирован из-за отсутствия активности пользователя более 30 секунд.",
                "Автоблокировка", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            foreach (Form openForm in Application.OpenForms.Cast<Form>().ToList())
            {
                if (!openForm.IsDisposed && openForm.Visible)
                {
                    openForm.Hide();
                }
            }

            Autorizathion loginForm = new Autorizathion();
            loginForm.ShowDialog();

            foreach (Form openForm in Application.OpenForms.Cast<Form>().ToList())
            {
                if (!openForm.IsDisposed)
                {
                    openForm.Show();
                }
            }

            if (currentActiveForm != null && !currentActiveForm.IsDisposed)
            {
                currentActiveForm.Focus();
            }

            ResumeMonitoring();
        }

        private class UserActionTracker : IMessageFilter
        {
            private const int WM_MOUSEMOVE = 0x0200;
            private const int WM_KEYDOWN = 0x0100;
            private const int WM_LBUTTONDOWN = 0x0201;
            private const int WM_RBUTTONDOWN = 0x0204;
            private const int WM_MBUTTONDOWN = 0x0207;

            public bool PreFilterMessage(ref Message m)
            {
                switch (m.Msg)
                {
                    case WM_MOUSEMOVE:
                    case WM_KEYDOWN:
                    case WM_LBUTTONDOWN:
                    case WM_RBUTTONDOWN:
                    case WM_MBUTTONDOWN:
                        RecordUserAction();
                        break;
                }
                return false;
            }
        }
    }
}
