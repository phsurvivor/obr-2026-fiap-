// Referências dos componentes
string motor_esquerda_ref = "me";
string motor_direita_ref = "md";
string sensor_cor_esquerda_ref = "scd"; // invertido pq funciona
string sensor_cor_esquerda_lado_ref = "scdl"; // invertido pq funciona
string sensor_cor_meio_ref = "scm";
string sensor_cor_direita_ref = "sce"; // invertido pq funciona
string sensor_cor_direita_lado_ref = "scel"; // invertido pq funciona
string ultrasonico_ref = "su";

// Configurações
bool dbg = false;
double delay_exec = 0.2;
double vel_padrao = 200;
double vel_padrao_curva = 1000;
double vel_padrao_curva2 = -700;
double angulo_descida_inf = 80.0;
double angulo_descida_sup = 90.0;
double vel_descida_fator = 0.2;
double angulo_subida = 270.0;
double vel_subida_fator = 1.5;

// Constantes de cores (compatível com inglês e português)
const string preto = "Black";
const string branco = "White";
const string vermelho = "Red";
const string verde = "Green";
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

// Métodos auxiliares para acessar componentes
Servomotor GetMotor(string referencia) => Bot.GetComponent<Servomotor>(referencia);
ColorSensor GetSensorCor(string referencia) => Bot.GetComponent<ColorSensor>(referencia);
UltrasonicSensor GetUltrasonic(string referencia) => Bot.GetComponent<UltrasonicSensor>(referencia);

// Movimentos
async Task AndarFrente(double velocidade = 100)
{
    GetMotor(motor_direita_ref).Locked = false;
    GetMotor(motor_esquerda_ref).Locked = false;
    GetMotor(motor_direita_ref).Apply(Math.Abs(velocidade), velocidade);
    GetMotor(motor_esquerda_ref).Apply(Math.Abs(velocidade), velocidade);
}

async Task Virar(double velocidade, double tick, bool ehDireita)
{
    string motorOposto = ehDireita ? motor_esquerda_ref : motor_direita_ref;
    string motorPrincipal = ehDireita ? motor_direita_ref : motor_esquerda_ref;

    GetMotor(motorOposto).Apply(900, vel_padrao_curva2);
    GetMotor(motorPrincipal).Locked = false;
    GetMotor(motorPrincipal).Apply(Math.Abs(velocidade * 2), velocidade * 2);
    await Time.Delay(tick);
}

async Task VirarEsquerda(double velocidade = 200, double tick = 0.9) => await Virar(velocidade, tick, false);
async Task VirarDireita(double velocidade = 200, double tick = 0.9) => await Virar(velocidade, tick, true);

async Task Volta(double velocidade = 100)
{
    GetMotor(motor_direita_ref).Locked = false;
    GetMotor(motor_esquerda_ref).Locked = false;
    GetMotor(motor_direita_ref).Apply(Math.Abs(-velocidade), -velocidade);
    GetMotor(motor_esquerda_ref).Apply(Math.Abs(-velocidade), -velocidade);
}

async Task TravarMotor()
{
    GetMotor(motor_direita_ref).Locked = true;
    GetMotor(motor_esquerda_ref).Locked = true;
}

async Task Virar2(double velocidade = 200, double tick = 1, bool ehDireita = true)
{
    if (dbg) IO.PrintLine(ehDireita ? "Direita" : "Esquerda");

    string motorOposto = ehDireita ? motor_esquerda_ref : motor_direita_ref;
    string motorPrincipal = ehDireita ? motor_direita_ref : motor_esquerda_ref;

    GetMotor(motorOposto).Apply(1000, ehDireita ? -700 : 0);
    GetMotor(motorPrincipal).Locked = false;
    GetMotor(motorPrincipal).Apply(Math.Abs(velocidade * 2), velocidade * 2);
    await Time.Delay(tick);
}

// Leitura de sensores
string LerSensorCor(string referencia) => GetSensorCor(referencia).Analog.ToString();

bool SensorEh(string referencia, string cor) => LerSensorCor(referencia) == cor;
bool SensorNaoEh(string referencia, string cor) => LerSensorCor(referencia) != cor;
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

// Ultrassônico: true se algum objeto está sendo visto pelos raios
bool ObjetoDetectado() => GetUltrasonic(ultrasonico_ref).Analog != -1;

// Distância do objeto captado, ou -1 se não houver nada
double DistanciaObjeto() => GetUltrasonic(ultrasonico_ref).Analog;

// Manobra de desvio: contorna o obstáculo e volta a avançar
async Task DesviarObstaculo(bool desviarDireita = true)
{
    if (dbg) IO.PrintLine("Obstaculo! Desviando...");

    // Para na frente do obstáculo
    await TravarMotor();
    await Time.Delay(150);

    // Lateral: sai do caminho do obstáculo
    if (desviarDireita) await VirarDireita(1000, 600);
    else await VirarEsquerda(1000, 600);

    // Avança paralelo ao obstáculo
    await AndarFrente(400);
    await Time.Delay(700);

    // Volta para a direção original
    if (desviarDireita) await VirarEsquerda(1000, 600);
    else await VirarDireita(1000, 600);

    // Passa reto pelo lado do obstáculo
    await AndarFrente(400);
    await Time.Delay(700);

    // Realinha
    if (desviarDireita) await VirarEsquerda(1000, 400);
    else await VirarDireita(1000, 400);

    await TravarMotor();
    if (dbg) IO.PrintLine("Desvio concluido");
}

async Task Main()
{
    if (dbg) IO.OpenConsole();

    while (true)
    {
        await Time.Delay(delay_exec);

        string sce = LerSensorCor(sensor_cor_esquerda_ref);
        string scd = LerSensorCor(sensor_cor_direita_ref);
        string scel = LerSensorCor(sensor_cor_esquerda_lado_ref);
        string scdl = LerSensorCor(sensor_cor_direita_lado_ref);

        if (dbg) IO.PrintLine($"{sensor_cor_esquerda_ref}: {sce} :: {sensor_cor_direita_ref}: {scd} :: {sensor_cor_esquerda_lado_ref}: {scel} :: {sensor_cor_direita_lado_ref}: {scdl}");

        // Obstáculo à frente - prioridade máxima: desviar antes de seguir a linha
        if (ObjetoDetectado())
        {
            if (dbg) IO.PrintLine($"Objeto detectado! Distancia: {DistanciaObjeto()}");

            // Escolhe o lado do desvio: se o sensor da esquerda está preto,
            // desvia para a direita (foge da borda); senão, para a esquerda
            bool desviarDireita = sce != preto;
            await DesviarObstaculo(desviarDireita);
            continue;
        }

        // Verde à esquerda - prioridade máxima
        if (scel == verde || sce == verde)
        {
            if (dbg) IO.PrintLine("Virar Esquerda (Verde)");
            await VirarEsquerda(1000, 1000);
        }
        // Verde à direita - prioridade máxima
        else if (scdl == verde || scd == verde)
        {
            if (dbg) IO.PrintLine("Virar Direita (Verde)");
            await VirarDireita(1000, 1000);
        }
        // Vermelho - parar
        else if (scd == vermelho || sce == vermelho)
        {
            if (dbg) IO.PrintLine("Travar");
            await TravarMotor();
        }
        // Ambos pretos - seguir em frente
        else if (scd == preto && sce == preto)
        {
            if (dbg) IO.PrintLine("Frente");
            await AndarFrente();
            await andar_frente(acelaracao_por_angulo(vel_padrao));
        }
        // Direita preta, esquerda não - virar direita
        else if (scd == preto && sce != preto)
        {
            if (dbg) IO.PrintLine("Virar Direita");
            await VirarDireita(1000);
        }
        // Esquerda preta, direita não - virar esquerda
        else if (sce == preto && scd != preto)
        {
            if (dbg) IO.PrintLine("Virar Esquerda");
            await VirarEsquerda(1000);
        }
        // Sensores laterais - ajuste fino
        else if (scdl == preto && scel != preto)
        {
            if (dbg) IO.PrintLine("Virar Direita 2");
            await Virar2(1000, 1000, true);
        }
        else if (scel == preto && scdl != preto)
        {
            if (dbg) IO.PrintLine("Virar Esquerda 2");
            await Virar2(1000, 1000, false);
        }
        // Caso padrão - seguir em frente
        else
        {
            if (dbg) IO.PrintLine("Frente");
            await AndarFrente(200);
            await andar_frente(acelaracao_por_angulo(200));
        }
    }
}
