# CandyCrushClone

## Descrição

Este projeto é um clone do popular jogo **Candy Crush**, desenvolvido no Unity. O objetivo é replicar as mecânicas principais do jogo original, como a seleção de peças, combinações e pontuação, além de oferecer ferramentas para facilitar o design e teste de fases.

O jogo conta com **3 fases pré-configuradas**, onde é possível ajustar diversos parâmetros, como tempo de jogo, pontuação das peças, dimensões da grid e peças disponíveis, tudo através de **ScriptableObjects**. Isso permite que game designers façam ajustes sem a necessidade de alterar o código.

Além disso, o projeto utiliza técnicas de **Object Pooling** para otimizar o desempenho, tanto para as células da grid quanto para os efeitos visuais.

---

## Funcionalidades

- **Seleção de Peças:** Controle através de cliques do mouse.
- **Construção Dinâmica da Grid:** A grid é gerada dinamicamente com base nas configurações definidas no `LevelConfigSO`.
- **Distribuição Dinâmica de Peças:** As peças são distribuídas aleatoriamente na grid, respeitando as configurações do `LevelConfigSO`.
- **Carregamento Dinâmico de Fases:** As fases são carregadas automaticamente a partir dos ScriptableObjects na pasta `Resources/LevelConfig`.
- **Ferramentas de Debug:**
  - Exibição das coordenadas da grid durante a execução no Editor.
  - Configuração de parâmetros de fase (tempo, pontuação, peças, etc.) via ScriptableObjects.

---

## Pré-requisitos

Para executar este projeto, você precisará dos seguintes recursos:

- **Unity 2022.3.57f1** ou superior.
- **Dotween** (para animações e transições).
- **Input System** (para controle de entrada).
- **Sprites 2D** (para os visuais do jogo).

---

## Como Usar

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/freitagtiago/CandyCrushClone.git