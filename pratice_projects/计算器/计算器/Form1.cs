using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
namespace 计算器
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            info.AppendText("1");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            info.AppendText("0");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            info.AppendText("2");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            info.AppendText("3");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            info.AppendText("4");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            info.AppendText("5");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            info.AppendText("6");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            info.AppendText("7");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            info.AppendText("8");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            info.AppendText("9");
        }

        private void add_Click(object sender, EventArgs e)
        {
            info.AppendText("+");
        }

        private void clear_Click(object sender, EventArgs e)
        {
            info.Clear();
        }

        private void back_Click(object sender, EventArgs e)
        {
            if(info.Text.Length != 0)
            {
                info.Text = info.Text.Substring(0, info.Text.Length - 1);
            }
           
        }

        private void button14_Click(object sender, EventArgs e)
        {
            info.AppendText("-");
        }

        private void button13_Click(object sender, EventArgs e)
        {
            info.AppendText("×");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            info.AppendText("÷");
        }

        public string Transform(string s)
        {
            Stack num = new Stack();
            Stack op = new Stack();
            char[] formula = s.ToCharArray();
            foreach(char i in formula)
            {   //数字直接入栈
                if (!(i == '+' || i == '-' || i == '×' || i == '÷')) {
                    num.Push(i);
                }
                //如果操作符栈为空，直接入栈
                else if(op.Count==0)
                {
                    op.Push(i);
                }
                else
                {
                    switch (i)
                    {
                        case '+':
                            char peek =Convert.ToChar(op.Peek());
                            while (peek == '-')
                            {
                                num.Push(peek);
                                op.Pop();
                            }
                            if (peek == '×' || peek == '÷')
                            {
                                op.Push(peek);
                            }

                            break;
                        case '-':
                            break;
                        case '×':
                            break; 
                        case '÷':
                            break;
                    }
                        
                          
                        
                }
            }
            return "";
        }
        private void Calculate()
        {
            char[] formula_arr = info.Text.ToCharArray();
        }
    }
}