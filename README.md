### Sistema de Funções
Funções primárias responsáveis pelo funcionamento e movimentação do robô.


| Função | Descrição | Parametros | Retorno |
|:---|:---|:---:|:---:|
|andar_frente() | Move o robo com velocidade e torque variavel| ```Velocidade``` ```Torque```| -
| voltar() | Gira o robo no proprio eixo 180º| - | -|
| girar() | Gira o robo no angulo desejado | ```Angulo``` | - 
| virar() | Faz uma leve curva sem parar o robo para o lado desejado | ```lado ('E') ou ('D') ``` | -
| virar_2() | Faz uma leve curva sem parar o robo para o lado desejado, usada para ajustes finos| ```lado ('E') ou ('D') ``` | -
| desviar_obstaculo() | Executa o procedimento de desvio | - | -
| acelaracao_por_angulo() | Ajusta a velocidade com base na inclinacao: reduz em descidas (80-90) e aumenta 50% em subidas (~270) | ``` velocidade_desejada ```| ```velocidade``` 



### Nomenclatura de Sensores

| Nome | Descrição
|:---| ---|
| **me** | Motor Esquerdo
| **md** | Motor Direito
| **sce** | Sensor de cor Esquerdo
| **scd** | Sensor de cor Direito
| **scdl** | Sensor de cor Direito lateral
| **scdl** | Sensor de cor Esquerdo lateral
| **scm** | Sensor de cor Central
| **su**   | Sensor Ultrasônico



### Nomenclatura de variaveis

| Nome | Descrição | Tipo
|:---| --- | --- |
| **dbg** | Modo debug | ```boolean``` |
| **vel_padrao** | Velodidade dos motores para frente | ```double``` 
| **vel_padrao_curva** | Velocidade dos motores do lado de fora da curva | ```double``` 
| **vel_padrao_curva2** | Velocidade dos motores do lado de dentro da curva | ```double```
| **angulo_descida_inf** | Limite inferior (graus) da faixa de descida | ```double```
| **angulo_descida_sup** | Limite superior (graus) da faixa de descida | ```double```
| **vel_descida_fator** | Fator de redução da velocidade em descidas | ```double``` || **angulo_subida** | Ângulo (graus) mínimo para considerar subida | ```double```
| **vel_subida_fator** | Fator de aumento (boost) da velocidade em subidas | ```double```
| **delay_exec** | Delay entre cada execução | ```double```
| **tick** | Tempo que a função de curva fica ativa, em ms | ```double```
