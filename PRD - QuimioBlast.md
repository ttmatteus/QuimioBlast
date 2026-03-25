# PRD \- QuimioBlast

## História do jogo

Em um laboratório de pesquisa um espécime escapou da contenção e saiu de controle, todo o local foi evacuado com os sobreviventes às pressas e o protocolo de quarentena foi instaurado no perímetro. Agora o local se encontra tomado por experimentos raivosos e descontrolados.  
Você, um robô chamado RAL (Robô Alquimista de Limpeza), foi criado pelos que fugiram a tempo com a missão de recuperar o controle do laboratório. Armado com sua alquimia destrutiva, sua missão é erradicar os experimentos restantes e limpar o antigo laboratório.

## Requisitos funcionais (RF)

Tabela dos requisitos funcionais

| ID-RF | RF | Descrição |
| :---: | :---: | ----- |
| 01 | Personagem | O jogador age no jogo através do personagem RAL. |
| 02 | Mapa | O “mundo” do jogo será um mapa em 2D, com resolução de 32x32 pixels. |
| 03 | Movimentação | O jogador pode se mover em até 4 direções, usando (WASD/setas), não podendo ultrapassar os limites do mapa ou outros objetos configurados como sólidos. |
| 04 | PV/HP | O jogador, e os inimigos, terão pontos de vida (PV/HP) que ao serem levados a 0 ou menos ocasionam em morte, para o jogador, fim de jogo e exibição da tela de morte. |
| 05 | Ataque | O jogador poderá atacar os inimigos para diminuir seus PV’s, causando dano, consequentemente matando-os. O ataque poderá sofrer alterações a critério da sua árvore de poderes ou itens. |
| 06 | Interação com NPC’s | O jogador poderá interagir com a NPC ALI ao se aproximar dela e clicar na tecla “E”, assim obtendo diálogos da NPC. |
| 07 | Árvore de poderes | Ao passar de algumas salas, o jogador poderá receber um novo poder de acordo com sua árvore de poderes. Não será possível desfazer uma escolha já feita. |
| 08 | Coleta de itens | Ao estar próximo de itens coletáveis e clicar na tecla “E”, o jogador os obterá, adicionando-os ao seu inventário. |
| 09 | Inventário | O jogador possuirá um inventário, espaço para armazenamento de itens coletáveis ao longo do jogo, totalizando 5 espaços em seu inventário. |
| 10 | Usar itens | O jogador poderá clicar com o mouse em itens de seu inventário para usá-los, os removendo de seu inventário após isso. |
| 11 | Menu | Ao clicar na tecla “esc”, o jogo entra em pausa, a tela de menu é mostrada com suas opções: Retornar/Sair. |
| 12 | Inimigos | O jogador enfrentará inimigos ao longo do jogo, derrotando-os ao reduzir seu PV a 0 ou menos. |
| 13 | Tela de morte | Quando o jogador tiver seu PV reduzido a 0 ou menos, será exibido a tela de morte (ou tela de game over). |
| 14 | Caixa de diálogo | Os personagens do jogo apresentam suas falas por meio da caixa de diálogo, localizada abaixo na tela. |
| 15 | Tela de início | Ao abrir o jogo, será apresentada a tela de início, com as opções: Jogar/Créditos/Sair  |
| 16 | Tela de introdução | Ao clicar no botão Jogar da tela de início, o jogador verá a tela de introdução, contando a história introdutória do jogo por 20s. Ainda, o jogador poderá “pular” essa tela clicando no botão “espaço”. |
| 17 | Árvore de diálogo | A partir de certos eventos, como a escolha de poderes ou o tempo decorrido em uma sala, o diálogo com ALI será diferente. |
| 18 | IA de aproximação | Os inimigos devem ter uma IA dedicada a se aproximar do jogador ao detectá-lo. |
| 19 | Pathfinding | O jogador poderá clicar com o mouse para poder guiar o personagem a um local do mapa, com o personagem devendo se deslocar até o local no melhor caminho. |

## Requisitos não funcionais (RNF)

Tabela dos requisitos não funcionais

| ID \- RNF | Requisito não funcional | Descrição |
| :---: | :---: | ----- |
| 01 | Taxa de quadros | O jogo deve ter uma taxa de quadros igual ou superior a 30 FPS. |
| 02 | Separação em salas | O jogo e sua progressão serão divididos através de salas |
| 03 | Plataforma mínima | O jogo deve, no mínimo rodar em computadores windows 10 |
| 04 | Separação de telas | O jogo deve ter uma boa separação de suas telas. |
| 05 | Organização de inventário | Ao consumir ou obter novos itens, o jogo deve organizá-los de acordo com os espaços disponíveis. |

## Funcionalidades extras

* Documentos de história: o jogo terá papeis como parte dos itens coletáveis, com o jogador podendo lê-los para saber detalhes da história ou do próprio jogo.

## Stack Tecnológica

Ao que foi acordado com os membros do grupo até o momento, o projeto será feito em Unity com a linguagem C\#.  
