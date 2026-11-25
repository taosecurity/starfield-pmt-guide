// Copy buttons, a collapsible menu on phones, and the menu highlight for the part of the page in view.
(function () {
  var d = document.querySelector('.rail-d');
  var mq = window.matchMedia('(max-width: 760px)');
  function fit() { if (d) d.open = !mq.matches; }
  fit();
  if (mq.addEventListener) mq.addEventListener('change', fit);

  [].forEach.call(document.querySelectorAll('.cmd .copy'), function (b) {
    b.addEventListener('click', function () {
      var code = b.closest('.cmd').querySelector('pre').innerText;
      function done(ok) { b.textContent = ok ? 'Copied' : 'Press Ctrl+C'; setTimeout(function () { b.textContent = 'Copy'; }, 1800); }
      if (navigator.clipboard && navigator.clipboard.writeText) {
        navigator.clipboard.writeText(code).then(function () { done(true); }, function () { done(false); });
      } else { done(false); }
    });
  });

  var links = [].slice.call(document.querySelectorAll('.rail a[href^="#"], .toc a'));
  var seen = {};
  var targets = [];
  links.forEach(function (a) {
    var id = a.getAttribute('href').slice(1);
    if (!seen[id] && document.getElementById(id)) { seen[id] = 1; targets.push(document.getElementById(id)); }
  });
  var cur = null, queued = false;
  function update() {
    queued = false;
    if (!targets.length) return;
    var id = targets[0].id;
    for (var i = 0; i < targets.length; i++) { if (targets[i].getBoundingClientRect().top <= 100) id = targets[i].id; }
    if (id === cur) return;
    cur = id;
    links.forEach(function (a) { a.classList.toggle('active', a.getAttribute('href') === '#' + id); });
  }
  function queue() { if (!queued) { queued = true; requestAnimationFrame(update); } }
  window.addEventListener('scroll', queue, { passive: true });
  window.addEventListener('resize', queue);
  update();
})();
