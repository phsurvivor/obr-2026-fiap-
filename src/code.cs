// Referências Sensores

string motor_esquerda_ref = "me";
string motor_direita_ref = "md";
string sensor_cor_esquerda_ref = "sce";
string sensor_cor_esquerda_lado_ref = "scel";
string sensor_cor_meio_ref = "scm";
string sensor_cor_direita_ref = "scd";
string sensor_cor_direita_lado_ref = "scdl";


// Parametros

bool dbg = false; // modo de debug
double delay_exec = .2;
double vel_padrao = 200;
double vel_padrao_curva = 1000;
double vel_padrao_curva2 = -700;
double angulo_descida_inf = 80.0;
double angulo_descida_sup = 90.0;
double vel_descida_fator = 0.2;
double angulo_subida = 270.0;
double vel_subida_fator = 1.5;

// o sBotics se tiver em outra língua, os sensores vão reportar outra cor.. por algum motivo
// só vamos aceitar...

// // Inglês
// const string preto = "Black";
// const string branco = "White";
// const string vermelho = "Red";
// const string verde = "Green";

const string preto = "Preto";
const string branco = "Branco";
const string vermelho = "Vermelho";
const string verde = "Verde";

// Movimentos
async Task andar_frente(double velocidade = 100) {
    Bot.GetComponent<Servomotor>(motor_direita_ref).Locked = false; // Destrava o motor da direita
    Bot.GetComponent<Servomotor>(motor_esquerda_ref).Locked = false; // Destrava o motor da esquerda
    Bot.GetComponent<Servomotor>(motor_direita_ref).Apply(Math.Abs(velocidade), velocidade); //  velocidade de rotação e torque
    Bot.GetComponent<Servomotor>(motor_esquerda_ref).Apply(Math.Abs(velocidade), velocidade); // velocidade de rotação e torque
}
async Task virar_esquerda(double velocidade = 200, double tick = 0.9) {
    Bot.GetComponent<Servomotor>(motor_esquerda_ref).Apply(900, vel_padrao_curva2);
    Bot.GetComponent<Servomotor>(motor_direita_ref).Locked = false; // Destrava o motor da direita
    Bot.GetComponent<Servomotor>(motor_direita_ref).Apply(Math.Abs(velocidade * 2), velocidade * 2); //*
    await Time.Delay(tick);
}
// Mesclar o virar_direita e o virar_esquerda ele deve receber o parametro se é direita ou esquerda
async Task virar_direita(double velocidade = 200, double tick = 0.9) {
    Bot.GetComponent<Servomotor>(motor_direita_ref).Apply(900, vel_padrao_curva2);
    Bot.GetComponent<Servomotor>(motor_esquerda_ref).Locked = false; // Destrava o motor da esquerda
    Bot.GetComponent<Servomotor>(motor_esquerda_ref).Apply(Math.Abs(velocidade * 2), velocidade * 2);
    await Time.Delay(tick);
}

async Task volta(double velocidade = 100) {
    Bot.GetComponent<Servomotor>(motor_direita_ref).Locked = false;
    Bot.GetComponent<Servomotor>(motor_esquerda_ref).Locked = false;
    Bot.GetComponent<Servomotor>(motor_direita_ref).Apply(Math.Abs(0 - velocidade), 0 - velocidade);
    Bot.GetComponent<Servomotor>(motor_esquerda_ref).Apply(Math.Abs(0 - velocidade), 0 - velocidade);
}
async Task travar_motor()
{ // 
    Bot.GetComponent<Servomotor>(motor_direita_ref).Locked = true; // Trava o motor da direita
    Bot.GetComponent<Servomotor>(motor_esquerda_ref).Locked = true; // Trava o motor da esquerda
}

async Task virar_2(double velocidade = 200, double tick = 1, string lado = "D")
{
    if (lado.ToString() == "E")
    {
        if (dbg) IO.PrintLine("Esquerda");
        Bot.GetComponent<Servomotor>(motor_direita_ref).Apply(1000, -00);
        Bot.GetComponent<Servomotor>(motor_esquerda_ref).Locked = false; // Destrava o motor da esquerda
        Bot.GetComponent<Servomotor>(motor_esquerda_ref).Apply(Math.Abs(velocidade * 2), velocidade * 2);
        await Time.Delay(tick);
    }
    if (lado.ToString() == "D")
    {
        if (dbg) IO.PrintLine("Direita");
        Bot.GetComponent<Servomotor>(motor_esquerda_ref).Apply(1000, -700);
        Bot.GetComponent<Servomotor>(motor_direita_ref).Locked = false; // Destrava o motor da direita
        Bot.GetComponent<Servomotor>(motor_direita_ref).Apply(Math.Abs(velocidade * 2), velocidade * 2); //*
        await Time.Delay(tick);
    }
}

double acelaracao_por_angulo(double velocidade) {
    // Medicoes de debug: descida = 80-90, subida ~270
    double inclinacao = Bot.Inclination;
    if (dbg) IO.PrintLine($"Inclinacao: {inclinacao}");
    if (inclinacao >= angulo_descida_inf && inclinacao <= angulo_descida_sup) {
        return velocidade * vel_descida_fator;
    }
    if (inclinacao >= angulo_subida) {
        return velocidade * vel_subida_fator;
    }
    return velocidade;
}

async Task Main()
{
    if (dbg) IO.OpenConsole();
    while (true)
    {
        await Time.Delay(delay_exec);

        string info_sensor_cor_esquerda = Bot.GetComponent<ColorSensor>(sensor_cor_esquerda_ref).Analog.ToString();
        string info_sensor_cor_direita = Bot.GetComponent<ColorSensor>(sensor_cor_direita_ref).Analog.ToString();
        string info_sensor_cor_esquerda_lado = Bot.GetComponent<ColorSensor>(sensor_cor_esquerda_lado_ref).Analog.ToString();
        string info_sensor_cor_direita_lado = Bot.GetComponent<ColorSensor>(sensor_cor_direita_lado_ref).Analog.ToString();

        if (dbg) IO.PrintLine($"{sensor_cor_esquerda_ref}: {info_sensor_cor_esquerda} :: {sensor_cor_direita_ref}: {info_sensor_cor_direita} :: {sensor_cor_esquerda_lado_ref}: {info_sensor_cor_esquerda_lado} :: {sensor_cor_direita_lado_ref}: {info_sensor_cor_direita_lado}");

        if (
            (info_sensor_cor_direita == preto) &&
            (info_sensor_cor_esquerda != preto)
        )
        {
            if (dbg) IO.PrintLine("Virar Direita");
            await virar_direita(1000);
        }
        else if (
            (info_sensor_cor_direita != preto) &&
            (info_sensor_cor_esquerda == preto)
        )
        {
            if (dbg) IO.PrintLine("Virar Esquerda");
            await virar_esquerda(1000);
        }
        else if (
            (info_sensor_cor_direita == preto) &&
            (info_sensor_cor_esquerda == preto)
        )
        {
            if (dbg) IO.PrintLine("Frente");
            await andar_frente(acelaracao_por_angulo(vel_padrao));
        }
        else if (
            (info_sensor_cor_direita_lado == preto) &&
            (info_sensor_cor_esquerda_lado != preto)
        )
        {
            if (dbg) IO.PrintLine("Virar Esquerda 2");
            await virar_2(1000, 1000, "E");
        }
        else if (
            (info_sensor_cor_direita_lado != preto) &&
            (info_sensor_cor_esquerda_lado == preto)
        )
        {
            if (dbg) IO.PrintLine("Virar Direita 2");
            await virar_2(1000, 1000, "D");
        }
        else if (
            (info_sensor_cor_direita == vermelho) ||
            (info_sensor_cor_esquerda == vermelho)
        )
        {
            if (dbg) IO.PrintLine("Travar");
            await travar_motor();
        }
        else
        {
            if (dbg) IO.PrintLine("Frente");
            await andar_frente(acelaracao_por_angulo(200));
        }
    }
}