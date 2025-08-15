// Simple confetti animation
let confettiCanvas = null;
let confettiCtx = null;
let confettiParticles = [];
let confettiAnimationId = null;

window.startConfetti = function() {
    // Create canvas for confetti
    confettiCanvas = document.createElement('canvas');
    confettiCanvas.style.position = 'fixed';
    confettiCanvas.style.top = '0';
    confettiCanvas.style.left = '0';
    confettiCanvas.style.width = '100vw';
    confettiCanvas.style.height = '100vh';
    confettiCanvas.style.pointerEvents = 'none';
    confettiCanvas.style.zIndex = '9999';
    confettiCanvas.width = window.innerWidth;
    confettiCanvas.height = window.innerHeight;
    
    document.body.appendChild(confettiCanvas);
    confettiCtx = confettiCanvas.getContext('2d');
    
    // Create particles
    confettiParticles = [];
    const colors = ['#ff6b6b', '#4ecdc4', '#45b7d1', '#a29bfe', '#fd79a8', '#feca57', '#fdcb6e', '#e17055'];
    
    for (let i = 0; i < 150; i++) {
        confettiParticles.push({
            x: Math.random() * window.innerWidth,
            y: Math.random() * window.innerHeight - window.innerHeight,
            vx: (Math.random() - 0.5) * 6,
            vy: Math.random() * 3 + 2,
            color: colors[Math.floor(Math.random() * colors.length)],
            rotation: Math.random() * 360,
            rotationSpeed: (Math.random() - 0.5) * 10,
            size: Math.random() * 8 + 4,
            life: 1.0,
            decay: Math.random() * 0.02 + 0.01
        });
    }
    
    // Start animation
    animateConfetti();
    
    // Auto-stop after 4 seconds
    setTimeout(() => {
        window.stopConfetti();
    }, 4000);
};

function animateConfetti() {
    if (!confettiCanvas || !confettiCtx) return;
    
    confettiCtx.clearRect(0, 0, confettiCanvas.width, confettiCanvas.height);
    
    for (let i = confettiParticles.length - 1; i >= 0; i--) {
        const particle = confettiParticles[i];
        
        // Update position
        particle.x += particle.vx;
        particle.y += particle.vy;
        particle.vy += 0.3; // gravity
        particle.rotation += particle.rotationSpeed;
        particle.life -= particle.decay;
        
        // Remove if off screen or faded
        if (particle.y > window.innerHeight + 20 || particle.life <= 0) {
            confettiParticles.splice(i, 1);
            continue;
        }
        
        // Draw particle
        confettiCtx.save();
        confettiCtx.translate(particle.x, particle.y);
        confettiCtx.rotate(particle.rotation * Math.PI / 180);
        confettiCtx.globalAlpha = particle.life;
        confettiCtx.fillStyle = particle.color;
        confettiCtx.fillRect(-particle.size/2, -particle.size/2, particle.size, particle.size);
        confettiCtx.restore();
    }
    
    if (confettiParticles.length > 0) {
        confettiAnimationId = requestAnimationFrame(animateConfetti);
    }
}

window.stopConfetti = function() {
    if (confettiAnimationId) {
        cancelAnimationFrame(confettiAnimationId);
        confettiAnimationId = null;
    }
    
    if (confettiCanvas && confettiCanvas.parentNode) {
        confettiCanvas.parentNode.removeChild(confettiCanvas);
    }
    
    confettiCanvas = null;
    confettiCtx = null;
    confettiParticles = [];
};
