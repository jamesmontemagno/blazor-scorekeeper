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
    confettiCanvas.style.zIndex = '99999'; // Higher z-index to appear above modals
    confettiCanvas.width = window.innerWidth;
    confettiCanvas.height = window.innerHeight;
    
    document.body.appendChild(confettiCanvas);
    confettiCtx = confettiCanvas.getContext('2d');
    
    // Create particles - MORE PARTICLES for bigger impact!
    confettiParticles = [];
    const colors = [
        '#ff6b6b', '#4ecdc4', '#45b7d1', '#a29bfe', '#fd79a8', '#feca57', 
        '#fdcb6e', '#e17055', '#00b894', '#e84393', '#0984e3', '#6c5ce7',
        '#fdac41', '#ee5a52', '#00cec9', '#a85cf9', '#fd79a8', '#55a3ff'
    ];
    
    // Create multiple bursts from different positions for more impact
    const burstCount = 3;
    const particlesPerBurst = 100;
    
    for (let burst = 0; burst < burstCount; burst++) {
        const burstX = (window.innerWidth / (burstCount + 1)) * (burst + 1);
        const burstY = window.innerHeight * 0.3; // Start from upper portion
        
        for (let i = 0; i < particlesPerBurst; i++) {
            confettiParticles.push({
                x: burstX + (Math.random() - 0.5) * 100,
                y: burstY + (Math.random() - 0.5) * 50,
                vx: (Math.random() - 0.5) * 15, // More spread
                vy: Math.random() * 8 + 3, // More upward velocity
                color: colors[Math.floor(Math.random() * colors.length)],
                rotation: Math.random() * 360,
                rotationSpeed: (Math.random() - 0.5) * 20, // Faster rotation
                size: Math.random() * 12 + 6, // Bigger particles
                life: 1.0,
                decay: Math.random() * 0.015 + 0.008, // Longer lasting
                shape: Math.random() > 0.5 ? 'square' : 'circle' // Mix of shapes
            });
        }
    }
    
    // Start animation
    animateConfetti();
    
    // Auto-stop after 6 seconds (longer celebration)
    setTimeout(() => {
        window.stopConfetti();
    }, 6000);
};

function animateConfetti() {
    if (!confettiCanvas || !confettiCtx) return;
    
    confettiCtx.clearRect(0, 0, confettiCanvas.width, confettiCanvas.height);
    
    for (let i = confettiParticles.length - 1; i >= 0; i--) {
        const particle = confettiParticles[i];
        
        // Update position
        particle.x += particle.vx;
        particle.y += particle.vy;
        particle.vy += 0.4; // Slightly stronger gravity
        particle.rotation += particle.rotationSpeed;
        particle.life -= particle.decay;
        
        // Add some air resistance for more realistic movement
        particle.vx *= 0.99;
        
        // Remove if off screen or faded
        if (particle.y > window.innerHeight + 50 || particle.life <= 0) {
            confettiParticles.splice(i, 1);
            continue;
        }
        
        // Draw particle with enhanced effects
        confettiCtx.save();
        confettiCtx.translate(particle.x, particle.y);
        confettiCtx.rotate(particle.rotation * Math.PI / 180);
        confettiCtx.globalAlpha = particle.life;
        confettiCtx.fillStyle = particle.color;
        
        // Add glow effect for extra sparkle
        confettiCtx.shadowColor = particle.color;
        confettiCtx.shadowBlur = 8;
        
        if (particle.shape === 'circle') {
            confettiCtx.beginPath();
            confettiCtx.arc(0, 0, particle.size/2, 0, Math.PI * 2);
            confettiCtx.fill();
        } else {
            // Square/rectangle
            confettiCtx.fillRect(-particle.size/2, -particle.size/2, particle.size, particle.size);
        }
        
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

// Enhanced celebration mode for big wins!
window.startWinnerCelebration = function() {
    // Start the enhanced confetti
    window.startConfetti();
    
    // Add extra bursts every 800ms for maximum impact
    let burstCount = 0;
    const maxBursts = 4;
    
    const burstInterval = setInterval(() => {
        if (burstCount >= maxBursts || !confettiCanvas) {
            clearInterval(burstInterval);
            return;
        }
        
        // Add extra burst particles
        const colors = [
            '#ff6b6b', '#4ecdc4', '#45b7d1', '#a29bfe', '#fd79a8', '#feca57', 
            '#fdcb6e', '#e17055', '#00b894', '#e84393', '#0984e3', '#6c5ce7',
            '#fdac41', '#ee5a52', '#00cec9', '#a85cf9', '#fd79a8', '#55a3ff'
        ];
        
        // Create side bursts for extra excitement
        for (let side = 0; side < 2; side++) {
            const burstX = side === 0 ? window.innerWidth * 0.1 : window.innerWidth * 0.9;
            const burstY = window.innerHeight * 0.4;
            
            for (let i = 0; i < 50; i++) {
                confettiParticles.push({
                    x: burstX + (Math.random() - 0.5) * 80,
                    y: burstY + (Math.random() - 0.5) * 40,
                    vx: (Math.random() - 0.5) * 18,
                    vy: Math.random() * 10 + 4,
                    color: colors[Math.floor(Math.random() * colors.length)],
                    rotation: Math.random() * 360,
                    rotationSpeed: (Math.random() - 0.5) * 25,
                    size: Math.random() * 14 + 8,
                    life: 1.0,
                    decay: Math.random() * 0.012 + 0.006,
                    shape: Math.random() > 0.3 ? 'circle' : 'square'
                });
            }
        }
        
        burstCount++;
    }, 800);
};
