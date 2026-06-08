---
name: 5. Nova Funcionalidade - Inimigos
about: Sugira uma melhoria ou nova feature para o projeto
title: "[FEATURE] Criação de IA e Comportamento de Inimigos Básicos"
labels: enhancement
assignees: ''
---

### 📋 Resumo da Ideia
<!-- Descreva em uma frase o que você gostaria de ver no projeto -->
Adicionar inimigos básicos ao mapa com rotinas simples de patrulha e perseguição ao detectar o jogador.

### ⚠️ Problema que Resolve
<!-- Qual dor ou limitação essa feature endereça? -->
O mundo do jogo parece vazio e sem desafios. A falta de obstáculos ou ameaças ativas remove o senso de urgência e conquista do jogador.

### 💡 Solução Proposta
<!-- Descreva como você imagina que essa funcionalidade deveria funcionar -->
Criar um ator/objeto "Inimigo" que possui três estados básicos: Patrulha (anda entre pontos definidos), Perseguição (entra em alerta e corre atrás do jogador ao avistá-lo) e Ataque (causa dano ao jogador se estiver muito próximo).

### 🔄 Alternativas Consideradas
<!-- Existe outra forma de resolver o problema? Por que você prefere a solução proposta? -->
*   **Inimigos estáticos (obstáculos fixos):** Não geram a mesma dinâmica de perigo e imersão que uma inteligência artificial em movimento proporciona.

### ✅ Critérios de Aceitação
<!-- Como saberemos que essa feature foi implementada corretamente? Liste os critérios -->
- [ ] O inimigo perde o interesse e volta a patrulhar se o jogador se afastar demais (raio de fuga).
- [ ] O inimigo possui pontos de vida (HP) e é destruído/executa animação de morte ao chegar a zero.
- [ ] O contato ou ataque do inimigo reduz a vida do jogador corretamente.

### 🎨 Mockups / Referências
<!-- Se aplicável, adicione esboços, prints de referência ou links de inspiração -->
*Diagrama de Máquina de Estados Simples (Patrulha -> Alerta -> Perseguição -> Ataque).*

### 🔍 Contexto Adicional
<!-- Alguma outra informação que ajude a entender a solicitação? -->
O sistema deve ser feito de forma modular para que possamos criar novos tipos de inimigos herdando essa mesma lógica básica de IA.
