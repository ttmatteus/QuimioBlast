---
name: Nova Funcionalidade
about: Sugira uma melhoria ou nova feature para o projeto
title: "[FEATURE] Menu Principal e Menu de Pause"
labels: enhancement
assignees: ''
---

### 📋 Resumo da Ideia
<!-- Descreva em uma frase o que você gostaria de ver no projeto -->
Desenvolver a interface do Menu Principal (tela inicial) e o Menu de Pause durante o gameplay.

### ⚠️ Problema que Resolve
<!-- Qual dor ou limitação essa feature endereça? -->
Atualmente o jogo inicia diretamente no gameplay e não há uma forma amigável de pausar a partida, ajustar configurações ou fechar o jogo de maneira segura.

### 💡 Solução Proposta
<!-- Descreva como você imagina que essa funcionalidade deveria funcionar -->
Criar uma tela inicial com opções de "Iniciar Jogo", "Configurações" e "Sair". Durante o jogo, pressionar 'Esc' deve congelar a física/tempo do jogo e abrir um menu com "Continuar", "Configurações" e "Voltar ao Menu Principal".

### 🔄 Alternativas Consideradas
<!-- Existe outra forma de resolver o problema? Por que você prefere a solução proposta? -->
*   **Fechar o jogo direto pelo Alt+F4:** Não é uma boa prática de experiência do usuário (UX) e pode corromper dados salvos.

### ✅ Critérios de Aceitação
<!-- Como saberemos que essa feature foi implementada corretamente? Liste os critérios -->
- [ ] O botão "Iniciar Jogo" carrega a cena inicial corretamente.
- [ ] O Menu de Pause congela totalmente as ações e inputs do jogo ao fundo.
- [ ] O botão "Sair" fecha o aplicativo corretamente em builds executáveis.

### 🎨 Mockups / Referências
<!-- Se aplicável, adicione esboços, prints de referência ou links de inspiração -->
*Layout simples centralizado ou alinhado à esquerda com transições suaves de fade-in/fade-out.*

### 🔍 Contexto Adicional
<!-- Alguma outra informação que ajude a entender a solicitação? -->
O design visual deve seguir a paleta de cores e a identidade artística definida para o jogo.
