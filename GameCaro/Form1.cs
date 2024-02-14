using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCaro
{
    public partial class Form1 : Form
    {
        #region Properties 
        ChessBoardManager ChessBoard;
        SocketManager socket;
        #endregion

        public Form1()
        {
            InitializeComponent();
            ChessBoard = new ChessBoardManager(pnChessBoard, tbNamePlayer, picMark);

            ChessBoard.EndedGame += ChessBoard_EndedGame;
            ChessBoard.PlayerMarked += ChessBoard_PlayerMarked;   
            
            proCoolDown.Step = Cons.COOL_DOWN_STEP;
            proCoolDown.Maximum = Cons.COOL_DOWN_TIME;
            proCoolDown.Value = 0;
            tmrCoolDown.Interval = Cons.COOL_DOWN_INTERVAL;
            socket = new SocketManager();
            NewGame();
           
        }
        #region Methods
        void EndGame()
        {
            tmrCoolDown.Stop();
            pnChessBoard.Enabled = false;
            undoToolStripMenuItem.Enabled = false;
            MessageBox.Show("Kết thúc game");
        }

        void NewGame()
        {   proCoolDown.Value = 0;
            tmrCoolDown.Stop();
            undoToolStripMenuItem.Enabled=true; 
            ChessBoard.DrawChessBoard();
            
        }

        void Quit()
        {   
            Application.Exit();
        }

        void Undo()
        {
            ChessBoard.Undo();
        }
        void ChessBoard_PlayerMarked(object sender, EventArgs e)
        {
            tmrCoolDown.Start();
            proCoolDown.Value = 0;
        }
        

        void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame();
        }
       
        private void tmrCoolDown_Tick(object sender, EventArgs e)
        {
            proCoolDown.PerformStep();
            if (proCoolDown.Value >= proCoolDown.Maximum)
            {   
                tmrCoolDown.Stop();
                EndGame();
                
                
            }    
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn chắc chắn muốn thoát", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                e.Cancel = true;
            
        }

        private void btnLan_Click(object sender, EventArgs e)
        {
            socket.IP = tbIP.Text;

            if (!socket.ConnectSever())
            {
                socket.CreateSever();
            }
            else
            {
              socket.Send("Thông tin từ Client");
            }

            Thread listenThread = new Thread(() =>
            {
                Listen();
            });
            listenThread.IsBackground = true;
            listenThread.Start();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            tbIP.Text = socket.GetLocalIPv4(NetworkInterfaceType.Wireless80211);

            if (string.IsNullOrEmpty(tbIP.Text))
            {
                tbIP.Text = socket.GetLocalIPv4(NetworkInterfaceType.Ethernet);
            }
        }

        void Listen()
        {
            string data = (string)socket.Receive();

            MessageBox.Show(data);
        }
        #endregion




    }
}
