using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp1
{
    public class MainViewModel:INotifyPropertyChanged
    {
        Modbus.Device.ModbusIpMaster master;

        private ushort _value;
        private ushort _input;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ushort Value
        {
            get { return _value; }
            set {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            }
           
        }

        public ushort Input
        {
            get { return _input; }
            set
            {
                _input = value;
            }
        }


        public ICommand BtnCommand { get; set; }


        public MainViewModel()
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", 502);
            master = Modbus.Device.ModbusIpMaster.CreateIp(tcpClient);

            Task.Run(async () =>
            {
                while (true)
                {

                    await Task.Delay(1000);
                    ushort[] values = master.ReadHoldingRegisters(1, 0, 1);

                    Value = values[0];
                }

            });

            BtnCommand = new CommandBase() { DoExecute = DoBtnCommand };
            
        }

        private void DoBtnCommand (Object obj)
        {
            master.WriteSingleRegister(1, 1, Input);
        }

    }
}
