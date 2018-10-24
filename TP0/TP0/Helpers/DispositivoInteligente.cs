﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TP0.Helpers.ORM;

namespace TP0.Helpers
{
    public class DispositivoInteligente : Dispositivo
    {
        [NotMapped]
        public State Estado;
        [NotMapped]
        [JsonProperty]
        public ICollection<State> estadosAnteriores;
        [NotMapped]
        public Actuador act;

        public DispositivoInteligente()
        {

        }

        //cons para crear nuevos
        public DispositivoInteligente(string nom, string idnuevo, double kWxHoraNuevo, double mx, double mn)
        {
            KWxHora = kWxHoraNuevo;
            Nombre = nom;
            Codigo = idnuevo;
            Max = mx;
            Min = mn;
            estadosAnteriores = new List<State>();
            ConsumoAcumulado = 0;
            EsInteligente = true;
            Estado = null;
            //act = new Actuador(DispositivoID);
        }

        public DispositivoInteligente(int DIID)//para buscar en la DB + instanciar
        {
            using (var context = new DBContext())
            {
                var Disp = context.Dispositivos.Find(DIID);
                KWxHora = Disp.KWxHora;
                Nombre = Disp.Nombre;
                Codigo = Disp.Codigo;
                Max = Disp.Max;
                Min = Disp.Min;
                estadosAnteriores = new List<State>();
                ConsumoAcumulado = 0;
                EsInteligente = true;
                IDUltimoEstado = Disp.IDUltimoEstado;
                this.ActualizarUltimoEstado();
                UsuarioID = Disp.UsuarioID;
                DispositivoID = Disp.DispositivoID;
                //act = new Actuador(DispositivoID);

            }
        }

        public override void ActualizarUltimoEstado()
        {
                using (var db = new DBContext())
                {
                    var ultimoEstado = db.Estados.Find(IDUltimoEstado);
                    switch (ultimoEstado.Desc)
                    {
                        case "Apagado":
                            Estado = new Apagado(this);
                            Estado.StateID = ultimoEstado.StateID;
                            break;
                        case "Encendido":
                            Estado = new Encendido(this);
                            Estado.StateID = ultimoEstado.StateID;
                            break;
                        case "Ahorro":
                            Estado = new Ahorro(this);
                            Estado.StateID = ultimoEstado.StateID;
                            break;
                        default:
                            throw new Exception("Estado no reconocido");
                }
            }
        }

        public override State GetEstado()
        {
            return Estado;
        }
        public override List<State> GetEstados()
        {
            //Retorna los estados del dispositivo
            using (var db = new DBContext())
            {
                return db.Estados.Where(e => e.DispositivoID == DispositivoID).ToList();
            }
        }

        public override bool EstaEncendido()
        {
            return Estado is Encendido;
        }
        public override bool EstaApagado()
        {
            return Estado is Apagado ;
        }
        public override bool EnAhorro()
        {
            return Estado is Ahorro;
        }

        public override void Encender()
        {
            Estado.Encender(this);
        }
        public override void Apagar()
        {
            Estado.Apagar(this);
        }
        public override void AhorrarEnergia()
        {
            Estado.AhorrarEnergia(this);
        }

        public override double Consumo()
        {
            double acumuladoKw = 0;
            ConsumoAcumulado = 0;
            double tiempoTotal = 0;

            estadosAnteriores = GetEstados();
            foreach (State s in estadosAnteriores)
            {
                double c = 0;
                if (s.FechaFinal == new DateTime(3000, 1, 1)) //Si el estado no termino, se usa la fecha de ahora como la final
                    s.FechaFinal = DateTime.Now;

                switch (s.Desc)
                {
                    case "Encendido":
                        c = (s.FechaFinal - s.FechaInicial).Minutes;
                        tiempoTotal += c;
                        break;
                    case "Ahorro":
                        c = (s.FechaFinal - s.FechaInicial).Minutes / 2;
                        tiempoTotal += c;
                        break;
                    case "Apagado":
                        tiempoTotal = (s.FechaFinal - s.FechaInicial).Minutes;
                        break;
                }
                ConsumoAcumulado += c;
                acumuladoKw += c * KWxHora / 60;
            }
            ConsumoPromedio = acumuladoKw / tiempoTotal;
            return acumuladoKw;
        }

        public override double ConsumoEnHoras(double horas)
        {
            using (var db = new DBContext())
            {
                estadosAnteriores = db.Estados.Where(e => e.DispositivoID == DispositivoID).ToList();
            }
            DateTime fFinal = DateTime.Now;
            DateTime fInicial = fFinal.AddHours(-horas);
            double hs = Static.FechasAdmin.ConsumoHsTotalPeriodo(fInicial, fFinal, estadosAnteriores);
            return hs * KWxHora;
        }

        public override double ConsumoEnPeriodo(DateTime fInicial, DateTime fFinal)
        {
            using (var db = new DBContext())
            {
                estadosAnteriores = db.Estados.Where(e => e.DispositivoID == DispositivoID).ToList();
            }
            double hs = Static.FechasAdmin.ConsumoHsTotalPeriodo(fInicial, fFinal, estadosAnteriores);
            return hs * KWxHora;
        }

        public override void AgregarEstado(State est)
        {
            Estado = est; //dejar sirve para los cambios de estado cuando el disp esta en memoria
                          //asi evitar recurrir a la base
            using (var db = new DBContext())
            {
                db.Estados.Add(est); //Agrega el nuevo estado a la db
                db.SaveChanges();
                IDUltimoEstado = est.StateID;
                var DIact = db.Dispositivos.Find(DispositivoID);
                DIact.IDUltimoEstado = est.StateID;
                db.SaveChanges();
            }
        }

        public override DispositivoInteligente ConvertirEnInteligente(string marca)
        {
            throw new NotImplementedException();
        }

        public void AsignarActuadorHumedad()
        {
            act = new ActuadorHumedad(DispositivoID);
        }
        public void AsignarActuadorMovimiento()
        {
            act = new ActuadorMovimiento(DispositivoID);
        }
        public void AsignarActuadorTemperatura()
        {
            act = new ActuadorTemperatura(DispositivoID);
        }
        public void AsignarActuadorLuz()
        {
            act = new ActuadorLuz(DispositivoID);
        }
    }
}
